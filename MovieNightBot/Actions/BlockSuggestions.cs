using Discord.Commands;

namespace MovieNightBot.Actions {
	public class BlockSuggestions : AdminAction {

		[Command("block_suggestions")]
		[Summary("Toggles allowing suggestions. Send `on` to disallow suggestions, `off` to allow.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await ReplyAsync(
				"Suggestions are currently "
				+ (Database.Controller.Server.GetByGuildId(Context.Guild.Id).SuggestionsBlocked ? "" : "not ")
				+ "blocked. If you wish to change that, give value of on or off with the block_suggestions command.");

		}

		[Command("block_suggestions")]
		[Summary("Toggles allowing suggestions. Send `on` to disallow suggestions, `off` to allow.")]
		public async Task Execute([Summary("(on|off)")] string toggleValue) {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			if (toggleValue == null || toggleValue == string.Empty) {
				await Context.Channel.SendMessageAsync("Must give value of on or off for block_suggestions command");
				return;
			}

			bool blockSuggestions = false;

			toggleValue = toggleValue.ToLower();
			if (toggleValue == "on")
				blockSuggestions = true;
			else if (toggleValue == "off")
				blockSuggestions = false;
			else {
				await Context.Channel.SendMessageAsync($"Unknown option for block_suggestions: '{toggleValue}'");
				return;
			}

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.SuggestionsBlocked = blockSuggestions;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync("Server suggestions are now " + (blockSuggestions ? "blocked" : "allowed"));
		}

	}
}
