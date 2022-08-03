using Discord.Commands;

namespace MovieNightBot.Actions {
	public class SetMovieChannel : AdminAction {

		[Command("set_channel")]
		[Summary("Sets the channel the bot wil listen in. Default is top text channel in the server.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.ChannelId = Context.Channel.Id;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Bot channel updated to {Context.Channel.Name}");
		}
	}
}
