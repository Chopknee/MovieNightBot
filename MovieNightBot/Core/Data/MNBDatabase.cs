using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace MovieNightBot.Core.Data {

    class MNBDatabase {
        private DatabaseConfigFile DatabaseConfiguration;


        public void Test() {
            //Get the database connection file and load it.
            if (!Initialize()) {
                return;
            }

            MySqlConnection conn = new MySqlConnection(DatabaseConfiguration.ConnectionString);

            try {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                //string sql = "SHOW TABLES";
                //MySqlCommand cmd = new MySqlCommand(sql, conn);
                //MySqlDataReader rdr = cmd.ExecuteReader();

                //while (rdr.Read()) {
                //    Console.WriteLine(rdr[0] + " -- " + rdr[1]);
                //}

                //rdr.Close();
                Console.WriteLine("Success!");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(DatabaseConfiguration.ConnectionString);
                Console.WriteLine(ex.StackTrace);

            }
            conn.Close();
            Console.WriteLine("Done!");
        }

        public bool Initialize() {
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
