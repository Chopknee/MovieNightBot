using Discord.Commands;

namespace MovieNightBot.Actions {
	public class AllowTVShows : AdminAction {

		[Command("allow_tv_shows")]
		[Summary("Toggles whether to allow tv shows in the IMDB search results (on) or not (off).")]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) { // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;
			}

			await ReplyAsync(
				"TV shows are currently " 
				+ (Database.Controller.Server.GetByGuildId(Context.Guild.Id).TVShowsAllowed? "" : "not ") 
				+ "allowed. If you wish to change that, give value of on or off with the imdb_tv_shows command." );

			return;
		}

		[Command("allow_tv_shows")]
		[Summary("Toggles whether to allow tv shows in the IMDB search results (on) or not (off).")]
		public async Task Execute([Summary("(on|off)")] string toggleValue) {

			if (!IsAuthenticatedUser()) { // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;
			}

			if (toggleValue == null || toggleValue == string.Empty) {
				await ReplyAsync("Must give value of on or off for imdb_tv_shows command");
				return;
			}

			bool allowTVShows = false;

			toggleValue = toggleValue.ToLower();
			if (toggleValue == "on")
				allowTVShows = true;
			else if (toggleValue == "off")
				allowTVShows = false;
			else {
				await Context.Channel.SendMessageAsync($"Unknown option for allow_tv_shows: '{toggleValue}'");
				return;
			}
			
			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.TVShowsAllowed = allowTVShows;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Allow IMDB tv show search turned {toggleValue}");
		}
	}
}
