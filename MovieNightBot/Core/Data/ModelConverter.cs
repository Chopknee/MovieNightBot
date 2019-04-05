//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;


//using Discord;
//using Discord.Commands;
//using Newtonsoft.Json;
//using Discord.WebSocket;

//namespace MovieNightBot.Core.Data {

//    class ModelConverter {
//        public ModelConverter () {
//            //string fileText;
//            //using (System.IO.StreamReader file = new System.IO.StreamReader($"{Program.DataDirectory}/446135665927651330.txt")) {
//            //    fileText = file.ReadToEnd();
//            //}
//            //OldDat parsed = JsonConvert.DeserializeObject<OldDat>(fileText);
//            //ServerData sd = parsed.Convert();//Hopefully we get to this point..
//        }
//    }

//    public class OldDat {
//        //public string ServerName = "";
//        //public string ServerId = "";
//        //public string DateCreated = "";
//        //public int MovieVoteOptionCount = 0;
//        //public int TiebreakerMethod = 0;
//        //public string AdminRoleName = "";
//        //public int UserVoteLimit = 4;
//        //public List<OldMovie> waitingMovies;
//        //public List<OldMovie> watchedMovies;

//        //public ServerData Convert() {
//        //    ServerData sd = new ServerData( ServerId, ServerName, DateCreated);
//        //    foreach (OldMovie m in waitingMovies) {
//        //        sd.AddMovie(Movie.Conv(m, false));
//        //    }
//        //    foreach (OldMovie m in watchedMovies) {
//        //        sd.AddMovie(Movie.Conv(m, true));
//        //    }
//        //    sd.MovieVoteOptionCount = MovieVoteOptionCount;
//        //    sd.TiebreakerMethod = TiebreakerMethod;
//        //    sd.AdminRoleName = AdminRoleName;
//        //    sd.UserVoteLimit = UserVoteLimit;
//        //    return sd;
//        }

//    }

//    public class OldMovie {
//        //public int TotalVotes = 0;
//        //public int TimesUpForVote = 0;
//        //public string Title;
//        //public string suggestDate = "";
//        //public string watchedDate = "";
//    }
//}
