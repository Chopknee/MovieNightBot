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
    public class JSONServerModel: IMoviesServerData {
        //This is where all information movies is kept, the key is the id of the 'guild'.
        private Dictionary<ulong, ServerData> serverMovies = new Dictionary<ulong, ServerData>();

        public ServerData GetServerMovies ( SocketGuild guild) {
            try {
                if (!serverMovies.ContainsKey(guild.Id)) {
                    if (!LoadData(guild)) {
                        Program.Instance.Log(new LogMessage(LogSeverity.Error, "JSON Model", $"An unknown error ocurred while loading a server file."));
                    }
                }
                return serverMovies[guild.Id];
            } catch (Exception ex) {
                throw ex;
            }
        }

        /**
         * Attempts to load a server's movies file. If none exists, an empty one is generated.
         * string guildId
         * string guidlName
         */
        private bool LoadData ( SocketGuild guild ) {
            try {
                if (!File.Exists($"{Program.DataDirectory}/{guild.Id}.txt")) {
                    ServerData mc = new ServerData(guild);
                    serverMovies.Add(guild.Id, mc);
                    SaveData(guild);
                    return true;
                }
                string fileText;
                using (System.IO.StreamReader file = new System.IO.StreamReader($"{Program.DataDirectory}/{guild.Id.ToString()}.txt")) {
                    fileText = file.ReadToEnd();
                }
                if (fileText.Equals("")) {
                    ServerData mc = new ServerData(guild);
                    serverMovies.Add(guild.Id, mc);
                    SaveData(guild);
                    return true;
                }
                ServerData parsed = JsonConvert.DeserializeObject<ServerData>(fileText);
                serverMovies.Add(guild.Id, parsed);
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Program.Instance.Log(new LogMessage(LogSeverity.Error, "JSON Model", $"An unknown error ocurred while loading a server file.", ex));
                return false;
            }
        }

        /**
         * Saves the specified server's movies file.
         * string guildId
         * string guidlName
         * return bool
         */
        private void SaveData ( SocketGuild guild ) {
            try {
                File.WriteAllText($"{Program.DataDirectory}/{guild.Id}.txt", JsonConvert.SerializeObject(serverMovies[guild.Id], Formatting.Indented));
                //return true;
            } catch (Exception ex) {
                throw ex;
            }
        }

        private void SaveData ( ulong GuildId ) {
            try {
                if (GuildId == 0) {
                    Console.WriteLine("For some reason guild id was 0");
                    throw new Exception("Guild Id was 0 for some reason.");
                }
                File.WriteAllText($"{Program.DataDirectory}/{GuildId}.txt", JsonConvert.SerializeObject(serverMovies[GuildId], Formatting.Indented));
            } catch (Exception ex) {
                throw ex;
            }
        }

        public ServerData GetServerData ( SocketGuild guild ) {
            try {
                ServerData serverCollection = GetServerMovies(guild);
                return serverCollection as ServerData;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void UpdateData ( ulong GuildId ) {
            try {
                SaveData(GuildId);
            } catch (Exception ex) {
                throw ex;
            }
        }
    }

    public class JSONDataException : DataException {}
}