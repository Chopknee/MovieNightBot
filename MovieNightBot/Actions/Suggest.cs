using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace MovieNightBot.Actions {
	public class Suggest : BaseAction {

		[Command("suggest")]
		[Summary("Adds the supplied movie to the suggestions list. There is a chance this movie will now show up on future votes.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute([Remainder][Summary("(Movie Name)")] string movieName) {

			if (!CheckForServerChannel())
				return;

			Database.Models.Server server = Database.Controller.Server.GetByGuildId(Context.Guild.Id);

			if (server.SuggestionsBlocked)
				await ReplyAsync("Suggestions are currently disabled on this server.");


			if (string.IsNullOrEmpty(movieName)) {
				await ReplyAsync("Must give movie name for suggest command.");
				return;
			}

			string sanitizedTitle = Util.CapitalizeMovieName(movieName);

			if (!server.MovieNamesChecked) {
				await SuggestNoSearch(sanitizedTitle);
			} else {
				//Also need to consider URLS
				System.Uri uri = null;
				bool isURI = System.Uri.TryCreate(movieName, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttps);
				if (!isURI) {
					await SuggestWithSearch(sanitizedTitle, server.TVShowsAllowed);
				} else {
					//Suggestion based on IMDB url.
					await SuggestByURL(uri);
				}
			}
		}

		//Simplest of the add methods. No IMDB searching.
		//Simply matching the title to an existing one, if not in the DB, add it.
		public async Task SuggestNoSearch(string cleanTitle, Database.Models.IMDBInfo? imdbInfo = null) {
			
			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Movie movieSearch = null;
				try {
					var servers = controller.Servers.Include(server => server.Movies);
					Database.Models.Server server = servers.Single(server => server.Id == Context.Guild.Id);
					if (server == null)
						throw new Exception("Server was not found.");

					movieSearch = server.Movies.Single(mov => mov.Name.Equals(cleanTitle));

				} catch { }

				if (movieSearch != null) {
					await ReplyAsync($"{cleanTitle} has already been suggested in this server.");
					return;
				}

				Database.Models.Movie movie = new Database.Models.Movie {
					Name = cleanTitle,
					ServerId = Context.Guild.Id,
					Suggestor = Context.User.Username,
					SuggestDate = new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds(),
					IMDBId = imdbInfo.Id
				};

				controller.Movies.Add(movie);
				await controller.SaveChangesAsync();
			}
			string year = imdbInfo != null ? $"({imdbInfo.ReleaseYear})" : string.Empty;
			await ReplyAsync($"Your suggestion of {cleanTitle} {year} has been added to the list.");
		}

		public async Task SuggestWithSearch(string cleanTitle, bool bIncludeShows) {
			Database.Models.IMDBInfo imdbInfo = await Util.SearchIMDBByTitle(cleanTitle, bIncludeShows);
			if (imdbInfo == null) {
				await ReplyAsync("Could not find the movie title you suggested in IMDb.");
				return;
			}

			await SuggestNoSearch(cleanTitle, imdbInfo);
		}

		public async Task SuggestByURL(System.Uri uri) {
			string[] split = uri.Segments;

			if (split.Length < 3) {
				await ReplyAsync("Could not find the movie title you suggested. The URL was invalid.");
				return;
			}

			Database.Models.IMDBInfo imdbInfo = await Util.GetIMDBInfo(split[2].TrimEnd('/'));

			if (imdbInfo == null) {
				await ReplyAsync("Could not find the movie title you suggested. The URL was invalid.");
				return;
			}

			await SuggestNoSearch(imdbInfo.Title, imdbInfo);
		}
	}
}
