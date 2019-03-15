using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Discord.WebSocket;

namespace MovieNightBot.Core.Data {

    //The data model responsible for handling movies.
    public class JSONServerModel : IMoviesServerData {
        //This is where all information movies is kept, the key is the id of the 'guild'.
        private Dictionary<string, JSONServerMovies> serverMovies = new Dictionary<string, JSONServerMovies>();
        private Random rand = new Random();//Used for the random vote generation

        /**
         * Checks if a movie has been either suggested, or watched.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public bool HasMovie(SocketGuild guild, string movieTitle) {
            return MovieHasBeenSuggested(guild, movieTitle) || MovieHasBeenWatched(guild, movieTitle);
        }

        /**
         * Checks if a movie has been suggested.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public bool MovieHasBeenSuggested(SocketGuild guild, string movieTitle) {
            JSONServerMovies movs = GetServerMovies(guild);
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
        public bool MovieHasBeenWatched(SocketGuild guild, string movieTitle) {
            JSONServerMovies movs = GetServerMovies(guild);
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
        public bool SetMovieToWatched(SocketGuild guild, string movieTitle, bool watched) {
            JSONServerMovies movs = GetServerMovies(guild);
            Movie theMovie = null;
            if (watched) {
                if (!MovieHasBeenWatched(guild, movieTitle)) {
                    for (int i = 0; i < movs.waitingMovies.Count; i++) {
                        if (movs.waitingMovies[i].Title.Equals(movieTitle)) {
                            theMovie = movs.waitingMovies[i];
                            movs.waitingMovies.RemoveAt(i);
                            movs.watchedMovies.Add(theMovie);
                            theMovie.watchedDate = DateTime.Now.ToString();
                            SaveData(guild);
                            return true;
                        }
                    }
                }
            } else {
                if (!MovieHasBeenSuggested(guild, movieTitle)) {
                    for (int i = 0; i < movs.waitingMovies.Count; i++) {
                        if (movs.watchedMovies[i].Title.Equals(movieTitle)) {
                            theMovie = movs.watchedMovies[i];
                            movs.watchedMovies.RemoveAt(i);
                            movs.waitingMovies.Add(theMovie);
                            theMovie.watchedDate = DateTime.Now.ToString();
                            SaveData(guild);
                            return true;
                        }
                    }
                }
            }
            //If execution reaches this point, we know something has gone terribly wrong.
            Console.WriteLine($"Attempting to set movie {movieTitle} to " + ((watched)? "watched" : "suggested") + " has failed.");
            return false;
        }

        /**
         * Removes a movie from the suggestions list.
         * string guildId
         * string guildName
         * string movieTitle
         * return bool
         */
        public bool RemoveMovie(SocketGuild guild, string movieTitle) {
            JSONServerMovies movs = GetServerMovies(guild);
            for (int i = 0; i < movs.waitingMovies.Count; i++) {
                if (movs.waitingMovies[i].Title.Equals(movieTitle)) {
                    movs.waitingMovies.RemoveAt(i);
                    SaveData(guild);
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
        public bool SetMovieToUnwatched(SocketGuild guild, string movieTitle) {
            JSONServerMovies movs = GetServerMovies(guild);
            Movie theMovie = null;
            for (int i = 0; i < movs.watchedMovies.Count; i++) {
                if (movs.watchedMovies[i].Title.Equals(movieTitle)) {
                    theMovie = movs.watchedMovies[i];
                    movs.watchedMovies.RemoveAt(i);
                    movs.waitingMovies.Add(theMovie);
                    theMovie.watchedDate = "";
                    SaveData(guild);
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
        public bool SuggestMovie(SocketGuild guild, string movieTitle) {
            JSONServerMovies movs = GetServerMovies(guild);
            movs.waitingMovies.Add(new Movie { Title = movieTitle, suggestDate = DateTime.Now.ToString(), watchedDate="" });
            SaveData(guild);
            return true;
        }

        /**
         * Function that returns the list of movies associated with the specified guild. If the server key is not in the dictionary,
         * load the file associated with that server.
         * string guildId
         * string guildName
         * return MovieCollection
         */
        public JSONServerMovies GetServerMovies(SocketGuild guild) {
            Console.WriteLine($"Looking for server {guild.Id}:{guild.Name} data.");
            if (!serverMovies.ContainsKey(guild.Id.ToString())) {
                Console.WriteLine($"Server {guild.Id}:{guild.Name} is not loaded.");
                LoadData(guild);
            }
            Console.WriteLine($"Server {guild.Id}:{guild.Name} is now loaded.");
            return serverMovies[guild.Id.ToString()];
        }

        /**
         * Generates a random array of 5 movies pulled from the suggestions/wait list.
         * string guildId
         * string guildName
         * return Movie[]
         */
        public Movie[] GetRandomVote(SocketGuild guild) {
            JSONServerMovies serverMoviesCollection = GetServerMovies(guild);
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
        public Movie[] GetWatchedMovies(SocketGuild guild) {
            JSONServerMovies serverCollection = GetServerMovies(guild);
            return serverCollection.watchedMovies.ToArray();
        }

        /**
         * Returns an array of all suggested/waiting movies on the specified server.
         * string guildId
         * string guidlName
         * return Movie[]
         */
        public Movie[] GetSuggestedMovies(SocketGuild guild) {
            JSONServerMovies serverCollection = GetServerMovies(guild);
            return serverCollection.waitingMovies.ToArray();
        }

        public string GetAdminRoleName(SocketGuild guild) {
            JSONServerMovies collection = GetServerMovies(guild);
            return collection.AdminRoleName;
        }

        /**
         * Set the count of movies to vote on.
         * string guildId
         * string guidlName
         * int count
         */
        public bool SetMovieVoteCount(SocketGuild guild, int count) {
            ServerData serverCollection = GetServerMovies(guild);
            serverCollection.MovieVoteCount = count;
            SaveData(guild);
            return true;
        }

        public bool SetTiebreakerOption(SocketGuild guild, int option) {
            ServerData serverCollection = GetServerMovies(guild);
            serverCollection.TiebreakerMethod = option;
            SaveData(guild);
            return true;
        }

        /**
         * Attempts to load a server's movies file. If none exists, an empty one is generated.
         * string guildId
         * string guidlName
         */
        private bool LoadData(SocketGuild guild) {
            try {
                if (!File.Exists($"{Program.DataDirectory}/{guild.Id}.txt")) {
                    JSONServerMovies mc = new JSONServerMovies { ServerName = guild.Name, ServerId = guild.Id.ToString(), DateCreated = DateTime.Now.ToString() };
                    serverMovies.Add(guild.Id.ToString(), mc);
                    SaveData(guild);
                    return true;
                }
                string fileText;
                using (System.IO.StreamReader file = new System.IO.StreamReader($"{Program.DataDirectory}/{guild.Id.ToString()}.txt")) {
                    fileText = file.ReadToEnd();
                }
                if (fileText.Equals("")) {
                    JSONServerMovies mc = new JSONServerMovies { ServerName = guild.Name, ServerId = guild.Id.ToString(), DateCreated = DateTime.Now.ToString() };
                    serverMovies.Add(guild.Id.ToString(), mc);
                    SaveData(guild);
                    return true;
                }
                JSONServerMovies parsed = JsonConvert.DeserializeObject<JSONServerMovies>(fileText);
                serverMovies.Add(guild.Id.ToString(), parsed);
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
        private bool SaveData(SocketGuild guild) {
            try {
                File.WriteAllText($"{Program.DataDirectory}/{guild.Id}.txt", JsonConvert.SerializeObject(serverMovies[guild.Id.ToString()], Formatting.Indented));
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        public bool Initialized() {
            throw new NotImplementedException();
        }

        public bool InitializeNewServer(SocketGuild guild) {
            throw new NotImplementedException();
        }
    }

    public class JSONServerMovies : ServerData {

        //Holds all the movies right here. The JSON library I'm using can serialize lists (yay!)
        public List<Movie> waitingMovies = new List<Movie>();
        public List<Movie> watchedMovies = new List<Movie>();
    }

    public class DataException : Exception {}
}