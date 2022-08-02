using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;

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

			if (config == null) {
				Console.WriteLine("No valid config file was loaded. Please create one, then include the path to it with -c in the command line arguments.");
				return;
			}

			new Application().StartThread().GetAwaiter().GetResult();

			//Database.Controller.Init(config.db_url);
			//The main test server id is 536019646554439689 // 446135665927651330
			using (var controller = new Database.Controller(Util.GetFilePath(config.db_url))) {
				//Console.WriteLine(controller.Servers.Count());

				List<Database.Models.Server> servers = controller.Servers.ToList();
				for (int i = 0; i < servers.Count; i++) {
					Console.WriteLine(servers[i].Id);
				}
				//Console.WriteLine(controller.Movies.Count());

				//List<Database.Models.Movie> movies = controller.Movies.ToList();
				//for (int i = 0; i < movies.Count; i++) {
				//	Console.WriteLine(movies[i].Name);
				//}

				//List<Database.Models.Movie> movieList = controller.Movies.Where(m => m.ServerId == 536019646554439689).ToList();
				//for (int i = 0; i < movieList.Count; i++) {
				//	Console.WriteLine(movieList[i].Name + " " + movieList[i].Server.Id);
				//}

				//List<Database.Models.Movie> movieList = controller.Movies.Where(m => m.ServerId == 446135665927651330).ToList();
				//for (int i = 0; i < movieList.Count; i++) {
				//	Database.Models.IMDBInfo imdbdata = movieList[i].IMDB;
				//	Console.WriteLine(movieList[i].IMDBId + " " + imdbdata);
				//	Console.WriteLine(movieList[i].Name + " " + movieList[i].Server.Id + " " + ((imdbdata != null) ? imdbdata.Title : "No IMDB data linked."));
				//}

				//List<Database.Models.IMDBInfo> imdbMovies = controller.IMDBInfo.ToList();
				//for (int i = 0; i < imdbMovies.Count; i++) {
				//	Console.WriteLine(imdbMovies[i].Id + " " + imdbMovies[i].ThumbnailPosterURL + " " + imdbMovies[i].FullSizePosterURL);//
				//}

				//List<Database.Models.MovieGenre> movieGenres = controller.MovieGenres.ToList();
				//for (int i = 0; i < movieGenres.Count; i++) {
				//	Console.WriteLine(movieGenres[i].Id + " " + movieGenres[i].Genre);
				//}
			}
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