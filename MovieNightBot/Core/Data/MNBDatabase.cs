using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Discord.WebSocket;

namespace MovieNightBot.Core.Data {

    class MNBDatabase {

        private volatile static MySqlConnection Connection;
        private volatile static DatabaseConfigFile DatabaseConfiguration;
        private volatile static bool Initialized = false;

        public volatile static string SERVERS_TABLE_NAME = "SERVERS";

        public static MySqlConnection GetMySQLConnection() {
            Connection = new MySqlConnection(DatabaseConfiguration.ConnectionString);
            return Connection;
        }

        public static bool Initialize() {
            try {
                if (!File.Exists(Program.DatabaseConfigFile)) {
                    Console.WriteLine($"Cannot initialize database connection, config.txt is missing! Please look for a file named; {Program.DatabaseConfigFile}");
                    //Output the default empty database config file.
                    File.WriteAllText(Program.DatabaseConfigFile, JsonConvert.SerializeObject(new DatabaseConfigFile(), Formatting.Indented));
                    return false;
                }
                string fileText;
                using (System.IO.StreamReader file = new System.IO.StreamReader(Program.DatabaseConfigFile)) {
                    fileText = file.ReadToEnd();
                }
                if (fileText.Equals("")) {
                    Console.WriteLine($"Cannot initialize database connection, config.txt is empty! Please look for a file named; {Program.DatabaseConfigFile}");
                    File.WriteAllText(Program.DatabaseConfigFile, JsonConvert.SerializeObject(new DatabaseConfigFile(), Formatting.Indented));
                    return false;
                }

                DatabaseConfigFile dbFile = JsonConvert.DeserializeObject<DatabaseConfigFile>(fileText);
                Console.WriteLine(dbFile.ConnectionString);
                DatabaseConfiguration = dbFile;

                string db = $"server={DatabaseConfiguration.server};user={DatabaseConfiguration.user};database=;port={DatabaseConfiguration.port};password={DatabaseConfiguration.password}";

                //Doing this once each time movie night bot is initialized. Attempting to make the database.
                MySqlConnection conn = new MySqlConnection(db);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"create database if not exists {DatabaseConfiguration.database}", conn);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine("Attempt to create database result; " + rows);
                //Attemtping to use the newly created database
                cmd = new MySqlCommand($"use {DatabaseConfiguration.database}", conn);
                rows = cmd.ExecuteNonQuery();
                Console.WriteLine("Attempt to use database result; " + rows);
                //Attempting to create a new table
                cmd = new MySqlCommand($"create table if not exists {SERVERS_TABLE_NAME} " +
                    $"(ID VARCHAR(255) PRIMARY KEY, NAME VARCHAR(255), MOVIEVOTECOUNT INTEGER, TIEBREAKMETHOD INTEGER, DATECREATED VARCHAR(255), ADMINROLENAME VARCHAR(255))"
                    , conn);
                rows = cmd.ExecuteNonQuery();
                Console.WriteLine("Attempt to create servers table result; " + rows);

                conn.Close();
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }
    }

    [Serializable]
    class DatabaseConfigFile {
        public string server = "localhost";
        public string user = "root";
        public string database = "";
        public int port = 3306;
        public string password = "";

        public string ConnectionString {
            get {
                return $"server={server};user={user};database={database};port={port};password={password}";
            }
        }
    }
}
