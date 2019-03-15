//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using Discord.WebSocket;
//using MySql.Data.MySqlClient;

//namespace MovieNightBot.Core.Data {
//    class MYSQLMoviesModel : IMoviesServerData {

//        private bool initted = false;

//        public MYSQLMoviesModel() {
//            //Run initialization procedures
//            initted = MNBDatabase.Initialize();
//        }

//        public bool Initialized() {
//            return initted;
//        }

//        public bool InitializeNewServer(SocketGuild guild) {
//            try {
//                MySqlConnection conn = MNBDatabase.GetMySQLConnection();
//                string command = "";
//                int result = 0;
//                //Initialize a new table for this server and add a new row with this server's information.
//                conn.Open();
//                try {
//                    command = $"INSERT INTO {MNBDatabase.SERVERS_TABLE_NAME} " +
//                        $"(ID, NAME, MOVIEVOTECOUNT, TIEBREAKMETHOD, DATECREATED, ADMINROLENAME) " +
//                        $"VALUES ('{guild.Id.ToString()}','{guild.Name}', 5, 0, '{DateTime.Today.ToString()}', 'Movie Master')";
//                    result = new MySqlCommand(command, conn).ExecuteNonQuery();
//                    Console.WriteLine("New data insert num rows; " + result);
//                } catch (MySqlException ex) {
//                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
//                }

//                command = $"create table if not exists server_{guild.Id.ToString()} " +
//                    "(ID INT(32) AUTO_INCREMENT PRIMARY KEY, TITLE VARCHAR(255), SUGGESTDATE VARCHAR(255), WHATCHDATE VARCHAR(255), WATCHED BOOLEAN)";
//                result = new MySqlCommand(command, conn).ExecuteNonQuery();
//                Console.WriteLine("New server table initialization num rows; " + result);

//                conn.Close();
//                return false;
//            } catch (Exception ex) {
//                throw new DatabaseException(ex.Message, ex.StackTrace);
//            }
//        }

//        public string GetAdminRoleName(SocketGuild guild) {
//            try {
//                MySqlConnection conn = MNBDatabase.GetMySQLConnection();
//                conn.Open();
//                string command = $"SELECT ADMINROLENAME FROM SERVERS WHERE ID = '{guild.Id.ToString()}'";
//                MySqlCommand comm = new MySqlCommand(command, conn);
//                MySqlDataReader rdr = comm.ExecuteReader();
//                if (!rdr.HasRows) {
//                    throw new DatabaseException($"Server {guild.Id.ToString()} {guild.Name} is calling commands, but has no entry in the database.", "During get admin role name function!");
//                }
//                rdr.Read();
//                conn.Close();
//                return rdr.GetString("ADMINROLENAME");
//            } catch (Exception ex) {
//                throw new DatabaseException(ex.Message, ex.StackTrace);
//            }
//        }

//        public bool SuggestMovie(SocketGuild guild, string movieTitle) {
//            try {
//                MySqlConnection conn = MNBDatabase.GetMySQLConnection();
//                conn.Open();
//                string command = $"INSERT INTO server_{guild.Id.ToString()} (TITLE, SUGGESTDATE, WATCHED) VALUES ('{movieTitle}', '{DateTime.Today}', false)";
//                int res = new MySqlCommand(command, conn).ExecuteNonQuery();
//                Console.WriteLine("Suggest move was run, result is " + res);
//                conn.Close();
//                return true;
//            } catch (MySqlException ex) {
//                //Yeppers
//                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
//                //In reality, I don't actually know what might have happened.
//                throw new DatabaseException($"Server {guild.Id.ToString()} {guild.Name} is calling commands, but has no entry in the database.", "During get admin role name function!");
//            } catch (Exception ex) {
//                throw ex;//Pass the error down the line.
//            }
//        }

//        public Movie[] GetRandomVote(SocketGuild guild) {
//            throw new NotImplementedException();
//        }

//        public Movie[] GetSuggestedMovies(SocketGuild guild) {
//            try {
//                List<Movie> movies = new List<Movie>();
//                MySqlConnection conn = MNBDatabase.GetMySQLConnection();
//                string command = $"select * from server_{guild.Id.ToString()}";
//                conn.Open();
//                MySqlDataReader reader = new MySqlCommand(command, conn).ExecuteReader();
//                while (reader.Read()) {
//                    Movie m = new Movie();
//                    reader.
//                }
//                conn.Close();
//                return reader.HasRows;
//            } catch (MySqlException ex) {
//                //Yeppers
//                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
//                //In reality, I don't actually know what might have happened.
//                throw new DatabaseException($"Server {guild.Id.ToString()} {guild.Name} is calling commands, but has no entry in the database.", "During get admin role name function!");
//            } catch (Exception ex) {
//                throw ex;//Pass the error down the line.
//            }
//        }

//        public Movie[] GetWatchedMovies(SocketGuild guild) {
//            throw new NotImplementedException();
//        }

//        public bool HasMovie(SocketGuild guild, string movieTitle) {
//            try {
//                return watchedQuery(guild, movieTitle, 2);
//            } catch (MySqlException ex) {
//                //Yeppers
//                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
//                //In reality, I don't actually know what might have happened.
//                throw new DatabaseException($"Server {guild.Id.ToString()} {guild.Name} is calling commands, but has no entry in the database.", "During get admin role name function!");
//            } catch (Exception ex) {
//                throw ex;//Pass the error down the line.
//            }
//        }

//        public bool MovieHasBeenSuggested(SocketGuild guild, string movieTitle) {
//            try {
//                return watchedQuery(guild, movieTitle, 1);
//            } catch (MySqlException ex) {
//                //Yeppers
//                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
//                //In reality, I don't actually know what might have happened.
//                throw new DatabaseException($"Server {guild.Id.ToString()} {guild.Name} is calling commands, but has no entry in the database.", "During get admin role name function!");
//            } catch (Exception ex) {
//                throw ex;//Pass the error down the line.
//            }
//        }

//        public bool MovieHasBeenWatched(SocketGuild guild, string movieTitle) {
//            try {
//                return watchedQuery(guild, movieTitle, 0);
//            } catch (MySqlException ex) {
//                //Yeppers
//                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
//                //In reality, I don't actually know what might have happened.
//                throw new DatabaseException($"Server {guild.Id.ToString()} {guild.Name} is calling commands, but has no entry in the database.", "During get admin role name function!");
//            } catch (Exception ex) {
//                throw ex;//Pass the error down the line.
//            }
//        }

//        private bool watchedQuery(SocketGuild guild, string title, int mode) {
//            MySqlConnection conn = MNBDatabase.GetMySQLConnection();
//            string command = $"select ID from server_{guild.Id.ToString()} where TITLE = '{title}'";
//            conn.Open();
//            switch (mode) {
//                case 0:
//                    command += " and WATCHED = 1";
//                    break;
//                case 1:
//                    command += " and WATCHED = 0";
//                    break;
//            }
//            MySqlDataReader reader = new MySqlCommand(command, conn).ExecuteReader();
//            conn.Close();
//            return reader.HasRows;
//        }

//        public bool RemoveMovie(SocketGuild guild, string movieTitle) {
//            throw new NotImplementedException();
//        }

//        public bool SetMovieToWatched(SocketGuild guild, string movieTitle, bool watched) {
//            throw new NotImplementedException();
//        }

//        public bool SetMovieVoteCount(SocketGuild guild, int count) {
//            throw new NotImplementedException();
//        }

//        public bool SetTiebreakerOption(SocketGuild guild, int option) {
//            throw new NotImplementedException();
//        }
//    }

//    public class DatabaseException : DataException {

//        string mess = "";
//        string stack = "";

//        public DatabaseException(string message, string stack) {
//            mess = message;
//            this.stack = stack;
//        }
        
//        public override string Message {
//            get {
//                return mess;
//            }
//        }

//        public override string StackTrace {
//            get {
//                return stack;
//            }
//        }
//    }
//}
