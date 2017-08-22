using System;
using ACCommunication;
using static ACCommunication.Authentification;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Client
{
    public class AcService : IAcService
    {

        public Task<AuthInfo> AskToken()
        {
            return Authentification.requestAccessToken();
        }

        public IEnumerable<AnimeInfo> QueryYearAndSeason(AuthInfo token, int year, SeasonsEnum season)
        {
            return Authentification.browseYearAndSeason(token, year.ToString(), Helpers.StringifySeasonEnum(season));
        }

        public bool WriteComments(IEnumerable<AnimeComment> comments)
        {
            return Authentification.writeComments(comments);
        }

        public IEnumerable<Tuple<int,AnimeComment>> ReadComments()
        {
            return Authentification.readAllComments();
        }
    }
}
