using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace MovieNightBot.Core.Data {

    //The data model responsible for handling movies.
    public class ServerData {
        //This is where all information movies is kept, the key is the id of the 'guild'.
        private volatile static Dictionary<string, MovieCollection> serverMovies = new Dictionary<string, MovieCollection>();
        private volatile static Random rand = new Random();//Used for the random vote generation

        public const string ROLE_NAME = "Movie Master";

        /**
         * Checks if a movie has been either suggested, or watched.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public static bool HasMovie(string guildId, string guildName, string movieTitle) {
            return HasMovieBeenSuggested(guildId, guildName, movieTitle) || HasMovieBeenWatched(guildId, guildName, movieTitle);
        }

        /**
         * Checks if a movie has been suggested.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public static bool HasMovieBeenSuggested(string guildId, string guildName, string movieTitle) {
            MovieCollection movs = GetServerMovies(guildId, guildName);
            bool res = false;
            foreach (Movie m in movs.waitingMovies) {
                res = m.Title.Equals(movieTitle);
                if (res) { break; }
            }
            return res;
        }

        /**
         * Checks if a movie has been watched.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public static bool HasMovieBeenWatched(string guildId, string guildName, string movieTitle) {
            MovieCollection movs = GetServerMovies(guildId, guildName);
            bool res = false;
            foreach (Movie m in movs.watchedMovies) {
                res = m.Title.Equals(movieTitle);
                if (res) { break; }
            }
            return res;
        }

        /**
         * Sets the state of a movie to watched. The movie will not appear in future votes.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public static bool SetMovieToWatched(string guildId, string guildName, string movieTitle) {
            MovieCollection movs = GetServerMovies(guildId, guildName);
            Movie theMovie = null;
            for (int i = 0; i < movs.waitingMovies.Count; i++) {
                if (movs.waitingMovies[i].Title.Equals(movieTitle)) {
                    theMovie = movs.waitingMovies[i];
                    movs.waitingMovies.RemoveAt(i);
                    movs.watchedMovies.Add(theMovie);
                    theMovie.watchedDate = DateTime.Now.ToString();
                    SaveMoviesFile(guildId, guildName);
                    return true;
                }
            }
            //If execution reaches this point, we know something has gone terribly wrong.
            Console.WriteLine($"Attempting to set movie {movieTitle} to watched has failed. Could not find it in suggested movies.");
            return false;
        }

        /**
         * Removes a movie from the suggestions list.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public static bool RemoveMovie(string guildId, string guildName, string movieTitle) {
            MovieCollection movs = GetServerMovies(guildId, guildName);
            for (int i = 0; i < movs.waitingMovies.Count; i++) {
                if (movs.waitingMovies[i].Title.Equals(movieTitle)) {
                    movs.waitingMovies.RemoveAt(i);
                    SaveMoviesFile(guildId, guildName);
                    return true;
                }
            }
            //If execution reaches this point, we know something has gone terribly wrong.
            Console.WriteLine($"Attempting to set movie {movieTitle} to watched has failed. Could not find it in suggested movies.");
            return false;
        }

        /**
         * Puts a previously watched movie back into the wait list. The movie will appear in future votes.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public static bool SetMovieToUnwatched(string guildId, string guildName, string movieTitle) {
            MovieCollection movs = GetServerMovies(guildId, guildName);
            Movie theMovie = null;
            for (int i = 0; i < movs.watchedMovies.Count; i++) {
                if (movs.watchedMovies[i].Title.Equals(movieTitle)) {
                    theMovie = movs.watchedMovies[i];
                    movs.watchedMovies.RemoveAt(i);
                    movs.waitingMovies.Add(theMovie);
                    theMovie.watchedDate = "";
                    SaveMoviesFile(guildId, guildName);
                    return true;
                }
            }
            //Reaching this point means the move is not in the list
            Console.WriteLine($"Attempting to set movie {movieTitle} to suggestions has failed. Could not find it in suggested movies.");
            return false;
        }

        /**
         * Adds a movie to the suggestions list. This movie will show in future votes.
         * string serverId
         * string guildId
         * string guildName
         */
        public static void SuggestMovie(string serverId, string guildId, string guildName) {
            MovieCollection movs = GetServerMovies(serverId, guildId);
            movs.waitingMovies.Add(new Movie { Title = guildName, suggestDate = DateTime.Now.ToString(), watchedDate="" });
            SaveMoviesFile(serverId, guildId);
        }

        /**
         * Function that returns the list of movies associated with the specified guild. If the server key is not in the dictionary,
         * load the file associated with that server.
         * string guildId
         * string guildName
         * return MovieCollection
         */
        public static MovieCollection GetServerMovies(string guildId, string guildName) {
            Console.WriteLine($"Looking for server {guildId}:{guildName} data.");
            if (!serverMovies.ContainsKey(guildId)) {
                Console.WriteLine($"Server {guildId}:{guildName} is not loaded.");
                LoadMoviesFile(guildId, guildName);
            }
            Console.WriteLine($"Server {guildId}:{guildName} is now loaded.");
            return serverMovies[guildId];
        }

        /**
         * Generates a random array of 5 movies pulled from the suggestions/wait list.
         * string guildId
         * string guildName
         * return Movie[]
         */
        public static Movie[] GetMovieVote(string guildId, string guildName) {
            MovieCollection serverMoviesCollection = GetServerMovies(guildId, guildName);
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

        /**
         * Returns an array of all watched movies on the specified server.
         * string guildId
         * string guidlName
         * return Movie[]
         */
        public static Movie[] GetWatchedMovies(string guildId, string guildName) {
            MovieCollection serverCollection = GetServerMovies(guildId, guildName);
            return serverCollection.watchedMovies.ToArray();
        }

        /**
         * Returns an array of all suggested/waiting movies on the specified server.
         * string guildId
         * string guidlName
         * return Movie[]
         */
        public static Movie[] GetWaitingMovies(string guildId, string guildName) {
            MovieCollection serverCollection = GetServerMovies(guildId, guildName);
            return serverCollection.waitingMovies.ToArray();
        }

        /**
         * Set the count of movies to vote on.
         * string guildId
         * string guidlName
         * int count
         */
        public static void SetMovieVoteCount(string guildId, string guildName, int count) {
            MovieCollection serverCollection = GetServerMovies(guildId, guildName);
            serverCollection.MovieVoteCount = count;
            SaveMoviesFile(guildId, guildName);
        }

        /**
         * Attempts to load a server's movies file. If none exists, an empty one is generated.
         * string guildId
         * string guidlName
         */
        private static bool LoadMoviesFile(string guildId, string guildName) {
            try {
                if (!File.Exists($"{Program.DataDirectory}/{guildId}.txt")) {
                    MovieCollection mc = new MovieCollection { ServerName = guildName, ServerId = guildId, DateCreated = DateTime.Now.ToString() };
                    serverMovies.Add(guildId, mc);
                    SaveMoviesFile(guildId, guildName);
                    return true;
                }
                string fileText;
                using (System.IO.StreamReader file = new System.IO.StreamReader($"{Program.DataDirectory}/{guildId}.txt")) {
                    fileText = file.ReadToEnd();
                }
                if (fileText.Equals("")) {
                    MovieCollection mc = new MovieCollection { ServerName = guildName, ServerId = guildId, DateCreated = DateTime.Now.ToString() };
                    serverMovies.Add(guildId, mc);
                    SaveMoviesFile(guildId, guildName);
                    return true;
                }
                MovieCollection parsed = JsonConvert.DeserializeObject<MovieCollection>(fileText);
                serverMovies.Add(guildId, parsed);
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        /**
         * Saves the specified server's movies file.
         * string guildId
         * string guidlName
         * return bool
         */
        private static bool SaveMoviesFile(string guildId, string guildName) {
            try {
                File.WriteAllText($"{Program.DataDirectory}/{guildId}.txt", JsonConvert.SerializeObject(serverMovies[guildId], Formatting.Indented));
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }
    }

    /**
     * A simple object represingting a server's movies list.
     */
    [Serializable]
    public class MovieCollection {
        public string ServerName;
        public string ServerId;
        public string DateCreated;
        public int MovieVoteCount = 5;//By default set to 5
        //Holds all the movies right here. The JSON library I'm using can serialize lists (yay!)
        public List<Movie> waitingMovies = new List<Movie>();
        public List<Movie> watchedMovies = new List<Movie>();
    }

    /**
     * A simple movie object.
     */
    [Serializable]
    public class Movie {
        public string Title { get; set; }
        public string suggestDate { get; set; }
        public string watchedDate { get; set; }
    }
}