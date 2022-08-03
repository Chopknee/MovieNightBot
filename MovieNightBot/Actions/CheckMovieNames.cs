using Discord.Commands;

namespace MovieNightBot.Actions {
	public class CheckMovieNames : AdminAction {

		[Command("check_movie_names")]
		[Summary("Toggles checking suggestions against IMDB database before adding.\nSend `on` to turn on, `off` to turn off.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await ReplyAsync(
				"IMDB movie name checks are "
				+ (Database.Controller.Server.GetByGuildId(Context.Guild.Id).MovieNamesChecked ? "" : "not ")
				+ "enabled. If you wish to change that, give value of on or off with the check_movie_names command.");

		}

		[Command("check_movie_names")]
		[Summary("Toggles checking suggestions against IMDB database before adding.\nSend `on` to turn on, `off` to turn off.")]
		public async Task Execute([Summary("(on|off)")] string toggleValue) {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			if (toggleValue == null || toggleValue == string.Empty) {
				await Context.Channel.SendMessageAsync("Must give value of on or off for check_movie_names command");
				return;
			}

			bool checkMovieNames = false;

			toggleValue = toggleValue.ToLower();
			if (toggleValue == "on")
				checkMovieNames = true;
			else if (toggleValue == "off")
				checkMovieNames = false;
			else {
				await Context.Channel.SendMessageAsync($"Unknown option for check_movie_names: '{toggleValue}'");
				return;
			}

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.MovieNamesChecked = checkMovieNames;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"IMDB movie name checks are now {toggleValue}");
		}

	}
}
