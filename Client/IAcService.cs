using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ACCommunication.Authentification;

namespace Client
{
    public interface IAcService
    {
        Task<AuthInfo> AskToken();
        IEnumerable<AnimeInfo> QueryYearAndSeason(AuthInfo token, int year, SeasonsEnum season);
        bool WriteComments(IEnumerable<AnimeComment> comments);
        IEnumerable<Tuple<int,AnimeComment>> ReadComments();
    }

}
