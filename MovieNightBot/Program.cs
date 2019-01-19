using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;

namespace MovieNightBot {
    class Program {
        private DiscordSocketClient client;
        private CommandService Commands;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync() {
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
            client.Log += ClientLog;

            string Token = "";
            using (var Stream = new FileStream((Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Replace(@"bin\Debug\netcoreapp2.1", @"Data\Token.txt"), FileMode.Open, FileAccess.Read))
            using (var ReadToken = new StreamReader(Stream)) {
                Token = ReadToken.ReadToEnd();
            }
            Console.WriteLine(Token);

            await client.LoginAsync(TokenType.Bot, Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ClientMessageReceived(SocketMessage messageParam) {
            var Message = messageParam as SocketUserMessage;
            var Context = new SocketCommandContext(client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;

            if (!(Message.HasStringPrefix("m!", ref ArgPos) || Message.HasMentionPrefix(client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

            if (!Result.IsSuccess) Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a" +
                $" command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
        }

        private async Task ClientReady() {
            await client.SetGameAsync("Getting developed!");
        }

        private async Task ClientLog(LogMessage Message) {
            Console.WriteLine($"{DateTime.Now} st {Message.Source}] {Message.Message}");
        }
    }
}
