using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using System.Linq;
using Newtonsoft.Json;

namespace MovieNightBot.Core.Data {

    public class MoviesData {
        public static volatile IMoviesServerData Model;
    }

    public interface IMoviesServerData {
        ServerData GetServerData ( SocketGuild guild );
        void UpdateData ( ulong GuildId );
    }

    /**
     * A simple object represingting a server's movies list.
     */
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class ServerData {

        public static ServerData Get(SocketGuild guild) {
            try {
                return MoviesData.Model.GetServerData(guild);
            } catch (Exception ex) {
                throw ex;
            }
        }

        [NonSerialized]
        private Random rand;

        public ServerData ( SocketGuild guild ) {
            serverName = guild.Name;
            guildId = guild.Id;
            dateCreated = DateTime.Now.ToString();
        }

        public ServerData (string guildId, string serverName, string dateCreated) {
            this.serverName = serverName;
            this.guildId = ulong.Parse(guildId);
            this.dateCreated = dateCreated;
        }

        public ServerData() {
            //Console.Write("I was created from the normal constructor!");
            foreach (Movie m in movies) {
                m.OnDataModified += UpdateDataModel;
            }
        }

        public void UpdateDataModel() {
            try {
                MoviesData.Model.UpdateData(guildId);
            } catch (Exception ex) {
                throw ex;
            }
        }

        public string ServerName {
            get { return serverName; }
            set {
                serverName = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private string serverName;

        //public string ServerId;
        public ulong GuildId {
            get { return guildId; }
            set {
                guildId = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private ulong guildId;

        public string DateCreated {
            get { return dateCreated; }
            set {
                dateCreated = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private string dateCreated;

        public int MovieVoteOptionCount {
            get { return movieVoteOptionCount; }
            set {
                movieVoteOptionCount = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private int movieVoteOptionCount = 5;

        public int TiebreakerMethod {
            get { return tieBreakerMethod; }
            set {
                tieBreakerMethod = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private int tieBreakerMethod = 0;//0 = new vote, 1 = tiebreaker vote

        public string AdminRoleName {
            get { return adminRoleName; }
            set {
                //Console.WriteLine("We did something, I just don't know why we don't see anything.");
                adminRoleName = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private string adminRoleName = "Movie Master";

        public int UserVoteLimit {
            get { return userVoteLimit; }
            set {
                userVoteLimit = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private int userVoteLimit = 3;//Number of movies users are allowed to vote on

        public int MovieTimeHour {
            get { return movieTimeHour; }
            set {
                movieTimeHour = value;
                UpdateDataModel();
            }
        }
        [JsonProperty]
        private int movieTimeHour = 21;
        [JsonProperty]
        private List<Movie> movies = new List<Movie>();

        public Movie[] GetSuggestedMovies() {
            List<Movie> ms = new List<Movie>();
            foreach (Movie m in movies) {
               if (!m.Watched) { ms.Add(m); }
            }
            return ms.ToArray();
        }

        public Movie[] GetWatchedMovies () {
            List<Movie> ms = new List<Movie>();
            foreach (Movie m in movies) {
                if (m.Watched) { ms.Add(m); }
            }
            return ms.ToArray();
        }

        public bool MovieHasBeenWatched(string title) {
            foreach (Movie m in movies) {
                if (m.Title == title && m.Watched) {
                    return true;
                }
            }
            return false;
        }

        public bool MovieHasBeenSuggested ( string title ) {
            foreach (Movie m in movies) {
                if (m.Title == title && !m.Watched) {
                    return true;
                }
            }
            return false;
        }

        public bool MovieHasBeenSuggestedOrWatched ( string title ) {
            foreach (Movie m in movies) {
                if (m.Title == title) {
                    return true;
                }
            }
            return false;
        }

        public void AddMovie(string title) {
            Movie m = new Movie(this, title);
            movies.Add(m);
            UpdateDataModel();
            m.OnDataModified += UpdateDataModel;
        }

        public Movie GetMovie(string title) {
            foreach (Movie mov in movies) {
                if (mov.Title == title) {
                    return mov;
                }
            }
            return null;
        }

        public void RemoveMove(string title) {
            Movie m = GetMovie(title);
            if (m != null) {
                movies.Remove(m);
            }
            UpdateDataModel();
            m.OnDataModified -= UpdateDataModel;
        }

        public Movie[] GetMovieSelection(int count) {
            List<Movie> movs = new List<Movie>();
            IEnumerable<float> scores = from suggestion in GetSuggestedMovies()
                                       where suggestion.Watched == false
                                       select suggestion.ClassificationScore;
            float max = scores.Max();

            //Order the list based on the scoring system.
            IEnumerable<Movie> suggs = from suggestion in GetSuggestedMovies()
                                       where suggestion.Watched == false
                                       //let score = (suggestion.ClassificationScore == 0)? max : suggestion.ClassificationScore
                                       orderby ((suggestion.ClassificationScore == -1)? max : suggestion.ClassificationScore)//score
                                       select suggestion;
            //Randomly select the movie
            while (movs.Count < count && suggs.Count() > 0) {
                if (rand == null) { rand = new Random(); }
                float val = (float)rand.NextDouble();
                val = MathF.Sqrt(val);//Create a curved value to favor higher index numbers
                int ind = (int)(suggs.Count() * val);
                movs.Add(suggs.ElementAt(ind));
                string title = suggs.ElementAt(ind).Title;
                suggs = suggs.Where(m => m.Title != title);
            }

            return movs.ToArray();
        }

        private static float Max(float max, float inp) {
            return Math.Max(max, inp);
        }
    }

    /**
     * A simple movie object.
     */
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class Movie {

        public delegate void DataModified ();
        public DataModified OnDataModified;

        private ServerData myServer;

        public Movie(ServerData myServer, string title) {
            this.myServer = myServer;
            this.title = title;
            suggestDate = DateTime.Now.ToString();
        }

        public Movie () { }

        public string Title {
            get {
                return title;
            }
            set {
                title = value;
                OnDataModified?.Invoke();
            }
        }
        [JsonProperty]
        private string title = "";

        public string SuggestDate {
            get {
                return suggestDate;
            }
        }
        [JsonProperty]
        private string suggestDate = "";

        public string WatchedDate {
            get {
                return watchedDate;
            }
        }
        [JsonProperty]
        private string watchedDate = "";

        public int TotalVotes {
            get {
                return totalVotes;
            }
            set {
                totalVotes = value;
                OnDataModified?.Invoke();
            }
        }
        [JsonProperty]
        private int totalVotes = 0;

        public int TimesUpForVote {
            get {
                return timesUpForVote;
            }
            set {
                timesUpForVote = value;
                OnDataModified?.Invoke();
            }
        }
        [JsonProperty]
        private int timesUpForVote = 0;

        public float TotalScore {
            get {
                return totalScore;
            }
            set {
                totalScore = value;
                OnDataModified?.Invoke();
            }
        }
        [JsonProperty]
        private float totalScore = 0;

        public bool Watched {
            get {
                return watched;
            }
            set {
                watched = value;
                if (value) {
                    //watchedDate = DateTime.Now.ToString();
                } else {
                    //watchedDate = "";
                }
                OnDataModified?.Invoke();
            }
        }
        [JsonProperty]
        private bool watched = false;

        public float ClassificationScore {
            get {
                return CalculateClassificationScore();
            }
        }

        private float CalculateClassificationScore() {
            if (timesUpForVote == 0) {
                return -1;
            }
            return totalVotes * totalScore / timesUpForVote;
        }

        public string ToString() {
            string str = $"Title: {Title.PadRight(255)}, Vote Times: {TimesUpForVote}, Total Votes: {TotalVotes}, Total Score {TotalScore}, Classification Score: {ClassificationScore}";
            return str;
        }

    }

    public class DataException: Exception { }
}
