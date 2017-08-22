module Constants
    open FSharp.Data

    [<Literal>]
    let rootApiUri = "https://anilist.co/api/"
    [<Literal>]
    let clientCredUriPart = "auth/access_token"
    [<Literal>]
    let browseUriPart = "browse/anime"
    [<Literal>]
    let tokenResponseSample = 
        """{"access_token":"NR3M3vXgHK0kmluOcJVlTXvbGOg4yLhAVyf5If", "token_type":"bearer", "expires":1414034981, "expires_in":3600 }"""
    [<Literal>]
    let browseResponseSample = 
        """[{"id":20919,"title_romaji":"Gyakusatsu Kikan","title_english":"Genocidal Organ","description":"test","title_japanese":"\u8650\u6bba\u5668\u5b98","type":"Movie","start_date_fuzzy":20170203,"end_date_fuzzy":null,"season":171,"series_type":"anime","synonyms":["Project Itoh"],"genres":["Psychological","Sci-Fi"],"adult":false,"average_score":0,"popularity":1150,"updated_at":1492792910,"image_url_sml":"https:\/\/cdn.anilist.co\/img\/dir\/anime\/sml\/20919-M2rDCso4xCZb.jpg","image_url_med":"https:\/\/cdn.anilist.co\/img\/dir\/anime\/med\/20919-M2rDCso4xCZb.jpg","image_url_lge":"https:\/\/cdn.anilist.co\/img\/dir\/anime\/reg\/20919-M2rDCso4xCZb.jpg","image_url_banner":null,"total_episodes":1,"airing_status":"finished airing","tags":[]}]"""
    [<Literal>]
    let commentsSample = """12,Welcome to the NHK,Great"""
    let authReqParams = 
        FormValues [
            ("grant_type", "client_credentials");
            ("client_id", "<your API client id>"); 
            ("client_secret", "<your API client secret>")]
    let timeoutInMs = 2000
    let saveFileName = "animecomments.csv"
