using Discord.Commands;

namespace MovieNightBot.Actions {
	public class Watched : BaseAction {
		[Command("watched")]
		[Summary("Posts the watched movies link in chat. You can view all movies watched for this server by following the posted link.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!CheckForServerChannel()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await Context.Channel.SendMessageAsync($"Suggestions can be found at {Application.config.base_url}movies.html?server={Context.Guild.Id}&view=watched");
		}
	}
}