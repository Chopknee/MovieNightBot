using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;

namespace MovieNightBot {
    public class Program {
        private DiscordSocketClient client;
        private CommandService Commands;
        private static DateTime startDate;

        public static volatile string CurrentDirectory;
        public static volatile string DataDirectory;
        public static volatile string LogDirectory;
        public static volatile string TokenDirectory;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync() {
            //CurrentDirectory = Directory.GetCurrentDirectory();
            DataDirectory = "Data";
            LogDirectory = "Logs";
            TokenDirectory = DataDirectory + "/Token";
            //Ensure that needed directories have been created (prevents errors and user confusion)
            Directory.CreateDirectory(DataDirectory);
            Directory.CreateDirectory(LogDirectory);
            Directory.CreateDirectory(TokenDirectory);

            

            startDate = DateTime.Now;
            client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            client.MessageReceived += ClientMessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            client.Ready += ClientReady;
            client.Log += Log;

            string Token = "";
            //Robust token file checking, first look for the file.
            if (!File.Exists($"{TokenDirectory}/Token.txt")) {
                await Log(new LogMessage(LogSeverity.Error, "", $"{DateTime.Now} at Initialization] Token file missing. Please look in Data\\Token for the file named Token.txt. This is where your bot's token need to go."));
                //Attempt to make an empty token file.
                try {
                    File.WriteAllText($"{TokenDirectory}/Token.txt", "[PASTE BOT TOKEN OVER THIS TEXT]");
                } catch (Exception ex) {
                    await Log(new LogMessage(LogSeverity.Error, "", $"{DateTime.Now} at Initialization] {ex.Message}\n{ex.StackTrace}"));
                }
                return;
            }
            //Try to load the token file.
            try {
                using (System.IO.StreamReader file = new System.IO.StreamReader($"{TokenDirectory}/Token.txt")) { 
                    Token = file.ReadToEnd();
                }
            } catch (Exception ex) {
                await Log(new LogMessage(LogSeverity.Error, "", $"{DateTime.Now} at Initialization] {ex.Message}\n{ex.StackTrace}"));
            }
            if (Token.Equals("") || Token.Equals("[PASTE BOT TOKEN OVER THIS TEXT]")) {
                await Log(new LogMessage(LogSeverity.Error, "", $"{DateTime.Now} at Initialization] Token file is empty. Please look in Data\\Token for the file named Token.txt. This is where your bot's token need to go."));
                return;
            }


            Console.WriteLine("Token was loaded successfully; " + Token);

            await client.LoginAsync(TokenType.Bot, Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ClientMessageReceived(SocketMessage messageParam) {
            SocketUserMessage Message = messageParam as SocketUserMessage;
            SocketCommandContext Context = new SocketCommandContext(client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;

            if (!(Message.HasStringPrefix("m!", ref ArgPos) || Message.HasMentionPrefix(client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

            if (!Result.IsSuccess) await Log(new LogMessage(LogSeverity.Error, "", $"{DateTime.Now} at Commands] Something went wrong with executing a" +
                $" command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}"));
        }

        private async Task ClientReady() {
            await client.SetGameAsync("Confused? Use m!help");
        }

        private Task Log(LogMessage Message) {
            Console.WriteLine($"{DateTime.Now} st {Message.Source}] {Message.Message}");
            File.AppendAllText(LogDirectory + $"/{startDate.Day}-{startDate.Month}-{startDate.Year}.{startDate.Hour}.{startDate.Minute}.txt", Message.ToString() + "\n");
            return Task.CompletedTask;
        }
    }
}
