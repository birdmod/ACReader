namespace ACCommunication

open System.IO
open FSharp.Data
open Constants

module Authentification = 
    type TokenResponseProvider = JsonProvider<tokenResponseSample>
    type BrowseResponseProvider = JsonProvider<browseResponseSample>
    type AnimeCommentsProvider = CsvProvider<commentsSample, HasHeaders = false, Schema="Id (int),Title (string),Comment (string)">

    type ItemFromApi = BrowseResponseProvider.Root
    type ItemFromCsv = AnimeCommentsProvider.Row

    type AuthInfo = {
        AccessToken:string
        ExpirationDate:System.DateTime
    } with 
        static member Create token validPeriodInSec = {
            AccessToken = token
            ExpirationDate = System.DateTime.Now.AddSeconds (float validPeriodInSec) }
        member this.IsExpired = 
            System.DateTime.Now >= this.ExpirationDate

    type AnimeInfo = {
        Id:int
        TitleRomaji:string
        Genres:string
        TotalEpisodes:int
        LogoBytes:byte[]
    } with 
        static member Create id romaji genres totalep imgBytes = {
            Id = id; TitleRomaji = romaji; Genres = genres; TotalEpisodes = totalep; LogoBytes = imgBytes }

    type AnimeComment = {
        Id:int
        Title:string
        Comment:string
    } with static member Create id title comment = {
            Id = id; Title = title; Comment = comment }

    let private queryAccessToken = 
        async {
            let buildClientCredUri = Path.Combine(rootApiUri, clientCredUriPart)
            return! Http.AsyncRequestString(buildClientCredUri, body = authReqParams, timeout=timeoutInMs)
        }

    let requestAccessToken() = 
        async {
            let! tokenCallRes = queryAccessToken
            return TokenResponseProvider.Parse tokenCallRes |> fun res -> (AuthInfo.Create res.AccessToken res.ExpiresIn)
        }
        |> Async.StartAsTask

    let buildBrowseUri (token: AuthInfo) (year:string) (season:string) =
        let tokenParamUri (uri:string) = 
            System.String.Format("{0}?access_token={1}", uri, token.AccessToken)
        let yearParamUri (y:string) (uri:string) = 
            System.String.Format("{0}&year={1}", uri, y)
        let seasonParamUri (s:string) (uri:string) = 
            System.String.Format("{0}&season={1}", uri, s)
        let fullPageParamUri (uri:string) = 
            System.String.Format("{0}&full_page=true", uri)
 
        Path.Combine(rootApiUri, browseUriPart)
        |> tokenParamUri
        |> yearParamUri year
        |> seasonParamUri season
        |> fullPageParamUri

    let retrieveImgByteForAnimeId (id:int) (url:string) = 
        async {
            try
                let! logoResp = Http.AsyncRequest(url, timeout=timeoutInMs)
                return 
                    match logoResp.Body with
                    | Text _ -> (id, Array.empty)
                    | Binary bytes -> (id, bytes)
            with
                | _ -> return (id, Array.empty)
        }

    let browseYearAndSeason (token: AuthInfo) (year:string) (season:string) = 
        let browseUri = buildBrowseUri token year season

        let browseAnimeResponse = BrowseResponseProvider.Load browseUri
        let logoDico = 
            browseAnimeResponse 
            |> Seq.map (fun (x:ItemFromApi) -> retrieveImgByteForAnimeId x.Id x.ImageUrlLge) 
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Map

        browseAnimeResponse 
        |> Seq.map (fun (x:ItemFromApi) -> AnimeInfo.Create x.Id x.TitleRomaji (System.String.Join(", ", x.Genres)) x.TotalEpisodes logoDico.[x.Id])

    let writeComments(items:AnimeComment seq) = 
        let processedDataInCsv = 
            items
            |> Seq.map (fun item -> System.String.Format("{0},{1},{2}\n", item.Id, item.Title, item.Comment))

        try
            File.WriteAllLines (saveFileName, processedDataInCsv)
            true
        with
            | _ -> false

    let readAllComments() = 
        let allComments = 
            if File.Exists saveFileName then
                File.ReadAllText saveFileName
                |> AnimeCommentsProvider.ParseRows
                (* 
                    In nomine Patris et Filii et Spiritus Sancti I am so sorry to use a System.Tuple 
                    but communication between F# and C# lacks things. This or a new class
                 *)
                |> Seq.map (fun (x:ItemFromCsv) -> x.Id, AnimeComment.Create x.Id x.Title x.Comment)
            else 
                Seq.empty
        allComments