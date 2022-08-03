using Discord.Commands;
using System.Text.RegularExpressions;

namespace MovieNightBot.Actions {
	public class SetMovieTime : AdminAction {

		[Command("set_movie_time")]
		[Summary("Sets the time when the movie will be watched. Format must be HH:MM\n"
			+ "The time is in UTC time zone, so convert accordingly. Valid range is 0 - 23\n"
			+ "This time shows at the bottom of the vote embed.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await ReplyAsync(
				$"Movie Time is currently set as {Database.Controller.Server.GetByGuildId(Context.Guild.Id).MovieTime.ToString("HH:mm UTC")}."
				+" To set the time please specify a valid value with set_movie_time.");

			return;
		}

		[Command("set_movie_time")]
		[Summary("Sets the time when the movie will be watched. Format must be HH:MM\n"
			+ "The time is in UTC time zone, so convert accordingly. Valid range is 0 - 23\n"
			+ "This time shows at the bottom of the vote embed.")]
		public async Task Execute([Summary("(HH:MM)")] DateTime timeValue) {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			//if (timeValue == null || timeValue == string.Empty) {
			//	await ReplyAsync("Must give value of on or off for imdb_tv_shows command");
			//	return;
			//}

			//Regex regex = new Regex(@"^\d{1,2}:\d{2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

			//if (!regex.IsMatch(timeValue)) {
			//	await ReplyAsync("Movie time given in invalid format. Must be `HH:MM`");
			//	return;
			//}
			//System.DateTime time = DateTime.Parse(timeValue + " UTC");

			//Will need to convert accordingly

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.MovieTime = timeValue;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Movie Time is now set to {Database.Controller.Server.GetByGuildId(Context.Guild.Id).MovieTime.ToString("HH:mm UTC")}.");
		}
	}
}
