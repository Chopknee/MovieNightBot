using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
namespace MovieNightBot.Core.Data {

    public class MoviesData {
        public static volatile IMoviesServerData Model;
    }

    public interface IMoviesServerData {
        //Functions about movies specifically
        bool HasMovie(SocketGuild guild, string movieTitle);
        bool MovieHasBeenSuggested(SocketGuild guild, string movieTitle);
        bool MovieHasBeenWatched(SocketGuild guild, string movieTitle);
        bool SetMovieToWatched(SocketGuild guild, string movieTitle, bool watched);
        bool RemoveMovie(SocketGuild guild, string movieTitle);
        bool SuggestMovie(SocketGuild guild, string movieTitle);
        Movie[] GetRandomVote(SocketGuild guild);
        Movie[] GetWatchedMovies(SocketGuild guild);
        Movie[] GetSuggestedMovies(SocketGuild guild);
        string GetAdminRoleName(SocketGuild guild);

        //Functions about the server
        
        bool SetTiebreakerOption(SocketGuild guild, int option);
        bool SetAdminRoleName ( SocketGuild guild, string name );
        bool SetMovieOptionCount ( SocketGuild guild, int count );//Num movies on a vote
        bool SetVoteCount ( SocketGuild guild, int count );//Vote limit

        
        int GetMovieOptionCount ( SocketGuild guild );
        int GetVoteCount ( SocketGuild guild );

        bool Initialized();
        bool InitializeNewServer(SocketGuild guild);
    }

    /**
     * A simple object represingting a server's movies list.
     */
    [Serializable]
    public class ServerData {
        public string ServerName;
        public string ServerId;
        public string DateCreated;
        public int MovieVoteOptionCount = 5;//By default set to 5
        public int TiebreakerMethod = 0;//0 = new vote, 1 = tiebreaker vote
        public string AdminRoleName = "Movie Master";
        public int UserVoteLimit = 3;//Number of movies users are allowed to vote on
    }

    /**
     * A simple movie object.
     */
    [Serializable]
    public class Movie {
        public string Title { get; set; }
        public string suggestDate { get; set; }
        public string watchedDate { get; set; }
        public int TotalVotes;
        public int TimesUpForVote;
    }
}
