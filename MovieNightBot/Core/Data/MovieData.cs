using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace MovieNightBot.Core.Data {
    public class Movies {
        //This is where all information movies is kept
        private volatile static Dictionary<string, MovieCollection> serverMovies = new Dictionary<string, MovieCollection>();
        private volatile static Random rand = new Random();

        //Checks if the movie has either been watched or suggested
        public static bool HasMovie(string serverId, string serverName, string movieTitle) {
            return HasMovieBeenSuggested(serverId, serverName, movieTitle) || HasMovieBeenWatched(serverId, serverName, movieTitle);
        }

        //Checks if the movie has been suggested
        public static bool HasMovieBeenSuggested(string serverId, string serverName, string movieTitle) {
            MovieCollection movs = GetServerMovies(serverId, serverName);
            bool res = false;
            foreach (Movie m in movs.waitingMovies) {
                res = m.Title == movieTitle;
            }
            return res;
        }

        //Checks if the movie has been watched
        public static bool HasMovieBeenWatched(string serverId, string serverName, string movieTitle) {
            MovieCollection movs = GetServerMovies(serverId, serverName);
            bool res = false;
            foreach (Movie m in movs.watchedMovies) {
                res = m.Title == movieTitle;
            }
            return res;
        }

        public static void SuggestMovie(string serverId, string serverName, string movieTitle) {
            MovieCollection movs = GetServerMovies(serverId, serverName);
            movs.waitingMovies.Add(new Movie { Title = movieTitle, suggestDate = DateTime.Now.ToString(), watchedDate="" });
            SaveMoviesFile(serverId, serverName);
        }

        public static MovieCollection GetServerMovies(string serverId, string serverName) {
            //Gets the movies associated with the specified server
            Console.WriteLine($"Number of loaded server files] {serverMovies.Count}");
            //Loads it if necessarry
            if (!serverMovies.ContainsKey(serverId)) {
                LoadMoviesFile(serverId, serverName);
            }
            return serverMovies[serverId];
        }

        public static Movie[] GetMovieVote(string serverId, string serverName) {
            Movie[] movies = new Movie[5];
            List<Movie> movs = new List<Movie>();
            MovieCollection mc = GetServerMovies(serverId, serverName);
            movs.AddRange(mc.waitingMovies);
            for (int i = 0; i < Math.Min(mc.MovieVoteCount, movs.Count); i++) {
                int ind = (int)Math.Floor(rand.NextDouble() * movs.Count);
                movies[i] = movs[ind];
                movs.RemoveAt(ind);
            }
            return movies;
        }

        private static void LoadMoviesFile(string serverId, string serverName) {
            if (!File.Exists(@"ServerFiles\" + $"{serverId}.txt")) {
                Console.WriteLine($"No file for server {serverName} (ID:{serverId} exists! Creating blank file in stead.");
                //Create the required object
                MovieCollection mc = new MovieCollection{ ServerName = serverName, ServerId = serverId, DateCreated = DateTime.Now.ToString() };
                serverMovies.Add(serverId, mc);
                SaveMoviesFile(serverId, serverName);//Force the file to save for the first time
                return;
            }
            string fileText;
            using (System.IO.StreamReader file = new System.IO.StreamReader(serverId + serverName)) {
                fileText = file.ReadToEnd();
            }
            if (fileText == "") {
                Console.WriteLine($"File for server {serverName} (ID:{serverId} is empty! Creating blank file in stead.");
                MovieCollection mc = new MovieCollection { ServerName = serverName, ServerId = serverId, DateCreated = DateTime.Now.ToString() };
                serverMovies.Add(serverId, mc);
                SaveMoviesFile(serverId, serverName);
                return;
            }
            MovieCollection parsed = JsonConvert.DeserializeObject<MovieCollection>(fileText);
            serverMovies.Add(serverId, parsed);
        }

        private static void SaveMoviesFile(string serverId, string serverName) {
            Console.WriteLine("Saving a file!");
            File.WriteAllText(@"ServerFiles\" + $"{serverId}.txt", JsonConvert.SerializeObject(serverMovies[serverId]));
        }
    }

    [Serializable]
    public class MovieCollection {
        public string ServerName;
        public string ServerId;
        public string DateCreated;
        public int MovieVoteCount = 5;//By default set to 5
        //These fields are for the use of JSON when being converted into text objects. (I hope)
        public Movie[] MoviesInWaiting {
            get {
                return waitingMovies.ToArray();
            }
            set {
                waitingMovies.Clear();
                waitingMovies.AddRange(value);
            }
        }
        public Movie[] MoviesWatched {
            get {
                return watchedMovies.ToArray();
            }
            set {
                waitingMovies.Clear();
                waitingMovies.AddRange(value);
            }
        }
        //These are the active data fields. (They are not serializable so they won't be saved in the file)
        public List<Movie> waitingMovies = new List<Movie>();
        public List<Movie> watchedMovies = new List<Movie>();
    }

    [Serializable]
    public class Movie {
        public string Title { get; set; }
        public string suggestDate { get; set; }
        public string watchedDate { get; set; }
    }
}
/*
 * A server will have a list of;
 *  suggested movies
 *  currently voting movies? //Probs not save this
 *  watched movies
 * 
 */