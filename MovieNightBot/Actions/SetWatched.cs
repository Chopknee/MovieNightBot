using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace MovieNightBot.Actions {
	public class SetWatched : BaseAction {

		[Command("set_watched")]
		[Summary("Sets the specified movie as having been watched. This movie will not show up on future votes.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute([Remainder][Summary("(Movie Name)")] string movieName) {

			if (!CheckForServerChannel())
				return;

			//Need to look for a movie with a matching guild and 
			if (string.IsNullOrEmpty(movieName)) {
				await ReplyAsync("Must give movie name for set_watched command");
				return;
			}

			movieName = Util.CapitalizeMovieName(movieName);

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Movie movie = null;
				try {
					var servers = controller.Servers.Include(server => server.Movies);
					Database.Models.Server server = servers.Single(server => server.Id == Context.Guild.Id);
					if (server == null)
						throw new Exception("Server was not found.");

					movie = server.Movies.Single(mov => mov.Name.Equals(movieName));

				} catch {
					await ReplyAsync($"No movie titled {movieName} has been suggested");
					return;
				}

				if (movie.WatchedDate != null || movie.WatchedDate != 0) {
					await ReplyAsync($"{movieName} was already set to watched on {System.DateTimeOffset.FromUnixTimeSeconds(movie.WatchedDate.Value)} .");
					return;
				}

				movie.WatchedDate = new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds();
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"{movieName} has been set as watched and will no longer show up in future votes.");
		}
	}
}
