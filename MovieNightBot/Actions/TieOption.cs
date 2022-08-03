using Discord.Commands;

namespace MovieNightBot.Actions {
	public class TieOption : AdminAction {

		private static string[] tieOptions = { "breaker", "random" };

		[Command("tie_option")]
		[Summary("Sets how the bot handles tied votes."
			+ "\nOption `breaker` will make a new vote using only the tied movies(this is the default)."
			+ "\nOption `random` will make a new vote with a random selection of movies.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await ReplyAsync($"The current option is set to {Database.Controller.Server.GetByGuildId(Context.Guild.Id).TieOption}.");

			return;
		}

		[Command("tie_option")]
		[Summary("Sets how the bot handles tied votes."
			+ "\nOption `breaker` will make a new vote using only the tied movies(this is the default)."
			+ "\nOption `random` will make a new vote with a random selection of movies.")]
		public async Task Execute([Summary("(breaker|random)")] string option) {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			if (option == null || option == string.Empty) {
				await ReplyAsync("Must give value of on or off for tie_option command");
				return;
			}

			bool bValid = false;

			for (int i = 0; i < tieOptions.Length; i++) {
				if (option == tieOptions[i]) {
					bValid = true;
					break;
				}
			}

			if (!bValid) {
				await ReplyAsync($"Unknown tiebreaker option given: {option}");
				return;
			}
			
			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.TieOption = option;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Tiebreaker updated to {option}.");
		}
	}
}
