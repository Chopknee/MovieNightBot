using Discord.Commands;

namespace MovieNightBot.Actions {
	public class Suggested : ModuleBase<SocketCommandContext> {

		[Command("suggested")]
		[Summary("Posts the suggested movies link in chat. You can view all movies suggested for this server by following the posted link.")]
		public async Task Execute() {
			await Context.Channel.SendMessageAsync($"Suggestions can be found at {Application.config.base_url}movies.html?server={Context.Guild.Id}&view=suggested");
		}
	}
}