using Discord.Commands;

namespace MovieNightBot.Actions {
	public class MovieOptionCount : AdminAction {

		[Command("movie_option_count")]
		[Summary("Sets the number of movies that will show up on a vote.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await ReplyAsync(
				$"Current number of movies per vote is {Database.Controller.Server.GetByGuildId(Context.Guild.Id).MovieCountPerVote}."
				+ "To change it, use the command movie_option_count.");
		}

		[Command("movie_option_count")]
		[Summary("Sets the number of movies that will show up on a vote.")]
		public async Task Execute([Summary("2 to 25")] int count) {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			if (count  < 2 || count > 25) {
				await ReplyAsync("Failed to update: Number of movies per vote must be between 2 and 25, inclusive");
				return;
			}

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.MovieCountPerVote = count;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Number of movies per vote updated to {count}");
		}
	}
}
