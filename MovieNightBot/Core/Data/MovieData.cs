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

        public static void SetMovieToWatched(string serverId, string serverName, string movieTitle) {
            MovieCollection movs = GetServerMovies(serverId, serverName);
            Movie theMovie = null;
            for (int i = 0; i < movs.waitingMovies.Count; i++) {
                if (movs.waitingMovies[i].Title.Equals(movieTitle)) {
                    theMovie = movs.waitingMovies[i];
                    movs.waitingMovies.RemoveAt(i);
                    movs.watchedMovies.Add(theMovie);
                    theMovie.watchedDate = DateTime.Now.ToString();
                    SaveMoviesFile(serverId, serverName);
                    return;
                }
            }
            Console.WriteLine($"Attempting to set movie {movieTitle} to watched has failed. Could not find it in suggested movies.");
        }

        public static void SetMovieToUnwatched(string serverId, string serverName, string movieTitle) {
            MovieCollection movs = GetServerMovies(serverId, serverName);
            Movie theMovie = null;
            for (int i = 0; i < movs.watchedMovies.Count; i++) {
                if (movs.watchedMovies[i].Title.Equals(movieTitle)) {
                    theMovie = movs.watchedMovies[i];
                    movs.watchedMovies.RemoveAt(i);
                    movs.waitingMovies.Add(theMovie);
                    theMovie.watchedDate = "";
                    SaveMoviesFile(serverId, serverName);
                    return;
                }
            }
            Console.WriteLine($"Attempting to set movie {movieTitle} to suggestions has failed. Could not find it in suggested movies.");
        }

        public static void SuggestMovie(string serverId, string serverName, string movieTitle) {
            MovieCollection movs = GetServerMovies(serverId, serverName);
            movs.waitingMovies.Add(new Movie { Title = movieTitle, suggestDate = DateTime.Now.ToString(), watchedDate="" });
            SaveMoviesFile(serverId, serverName);
        }

        public static MovieCollection GetServerMovies(string serverId, string serverName) {
            //Gets the movies associated with the specified server
            Console.WriteLine($"Looking for server {serverId}:{serverName} data.");
            //Loads it if necessarry
            if (!serverMovies.ContainsKey(serverId)) {
                Console.WriteLine($"Server {serverId}:{serverName} is not loaded.");
                LoadMoviesFile(serverId, serverName);
            }
            Console.WriteLine($"Server {serverId}:{serverName} is now loaded.");
            return serverMovies[serverId];
        }

        public static Movie[] GetMovieVote(string serverId, string serverName) {
            MovieCollection serverMoviesCollection = GetServerMovies(serverId, serverName);
            List<Movie> allMovies = new List<Movie>();
            allMovies.AddRange(serverMoviesCollection.waitingMovies);
            int count = Math.Min(serverMoviesCollection.MovieVoteCount, allMovies.Count);
            Movie[] movies = new Movie[count];
            for (int i = 0; i < count; i++) {
                int randomIndex = rand.Next(0, allMovies.Count);
                movies[i] = allMovies[randomIndex];
                allMovies.RemoveAt(randomIndex);
            }
            return movies;
        }

        private static void LoadMoviesFile(string serverId, string serverName) {
            if (!File.Exists(@"ServerFiles\" + $"{serverId}.txt")) {
                MovieCollection mc = new MovieCollection{ ServerName = serverName, ServerId = serverId, DateCreated = DateTime.Now.ToString() };
                serverMovies.Add(serverId, mc);
                SaveMoviesFile(serverId, serverName);
                return;
            }
            string fileText;
            using (System.IO.StreamReader file = new System.IO.StreamReader(@"ServerFiles\" + $"{serverId}.txt")) {
                fileText = file.ReadToEnd();
            }
            if (fileText.Equals("")) {
                MovieCollection mc = new MovieCollection { ServerName = serverName, ServerId = serverId, DateCreated = DateTime.Now.ToString() };
                serverMovies.Add(serverId, mc);
                SaveMoviesFile(serverId, serverName);
                return;
            }
            MovieCollection parsed = JsonConvert.DeserializeObject<MovieCollection>(fileText);
            serverMovies.Add(serverId, parsed);
        }

        private static void SaveMoviesFile(string serverId, string serverName) {
            File.WriteAllText(@"ServerFiles\" + $"{serverId}.txt", JsonConvert.SerializeObject(serverMovies[serverId], Formatting.Indented));
        }
    }

    [Serializable]
    public class MovieCollection {
        public string ServerName;
        public string ServerId;
        public string DateCreated;
        public int MovieVoteCount = 5;//By default set to 5
        //These fields are for the use of JSON when being converted into text objects. (I hope)
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