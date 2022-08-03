using Discord.Commands;

namespace MovieNightBot.Actions {
	public class Watched : ModuleBase<SocketCommandContext> {
		[Command("watched")]
		[Summary("Posts the watched movies link in chat. You can view all movies watched for this server by following the posted link.")]
		public async Task Execute() {
			await Context.Channel.SendMessageAsync($"Suggestions can be found at {Application.config.base_url}movies.html?server={Context.Guild.Id}&view=watched");
		}
	}
}