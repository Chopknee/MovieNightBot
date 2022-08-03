using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;

/**
 * Main todo items:
 * Port remainder of commands
 * Determine best method for keeping database in memory for faster loading and manipulation. (while maintaining general safety of the data)
 *		Possibly done by loading the db file into memory on initial boot, then running read queries with the in-memory version, and writing after successful updates?
 * 
 */

namespace MovieNightBot {
	public class Application {

		public static string logOutputFilename = "";
		public static string configFilename = "";
		public static Config config = null;

		// Application entry point
		public static void Main(string[] args) {

			Console.WriteLine("MovieNightBot.Net starting up...");
			for (int i = 0; i < args.Length; i++) {
				if (args[i] == "-f") {
					i++;
					//Log output filename
					logOutputFilename = Util.GetFilePath(args[i]);
					Console.WriteLine("Log output filename " + logOutputFilename);

					continue;
				}

				if (args[i] == "-c") {
					i++;
					//Config file path
					configFilename = Util.GetFilePath(args[i]);
					Console.WriteLine("Config filename " + configFilename);
					continue;
				}
			}

			config = Config.Init(configFilename);
			Database.Controller.Init(config.db_url);

			if (config == null) {
				Console.WriteLine("No valid config file was loaded. Please create one, then include the path to it with -c in the command line arguments.");
				return;
			}

			//Start the discord portion of the bot.
			new Application().StartThread().GetAwaiter().GetResult();
		}

		private DiscordSocketClient client = null;
		private CommandHandler commandHandler = null;

		public async Task StartThread() {
			client = new DiscordSocketClient();
			client.Log += Log;

			await client.LoginAsync(Discord.TokenType.Bot, config.token);
			await client.StartAsync();

			await client.SetGameAsync("Tracking your shitty movie taste");


			CommandServiceConfig csc = new CommandServiceConfig();
			csc.DefaultRunMode = RunMode.Async;
			CommandService commandService = new CommandService(csc);

			commandHandler = new CommandHandler(client, commandService);
			await commandHandler.InstallCommandsAsync();


			await Task.Delay(-1);
		}

		private Task Log(Discord.LogMessage message) {
			Console.WriteLine(message.ToString());
			return Task.CompletedTask;

		}


		internal class CommandHandler {

			private readonly DiscordSocketClient client;
			private readonly CommandService commands;

			public CommandHandler(DiscordSocketClient client, CommandService commands) {
				this.client = client;
				this.commands = commands;
			}

			public async Task InstallCommandsAsync() {
				client.MessageReceived += HandleCommandAsync;

				await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
			}

			private async Task HandleCommandAsync(SocketMessage messageParam) {
				var message = messageParam as SocketUserMessage;

				if (message == null) return;

				int argPos = 0;

				if (!(message.HasStringPrefix(Application.config.message_identifier, ref argPos)) || message.Author.IsBot)
					return;

				var context = new SocketCommandContext(client, message);

				await commands.ExecuteAsync(
					context: context,
					argPos: argPos,
					services: null);
			}
		}
	}
}