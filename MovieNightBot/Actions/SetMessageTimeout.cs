using Discord.Commands;

namespace MovieNightBot.Actions {
	public class SetMessageTimeout : AdminAction {

		[Command("set_message_timeout")]
		[Summary("Sets how long before the suggestion messages are deleted, in seconds. Set to 0 to not delete them.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) { // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;
			}

			await ReplyAsync(
				$"Message timeout is currently {Database.Controller.Server.GetByGuildId(Context.Guild.Id).MessageTimeout}."
				+ " If you wish to chage it use set_message_timeout and specify a new value.");
		}

		[Command("set_message_timeout")]
		[Summary("Sets how long before the suggestion messages are deleted, in seconds. Set to 0 to not delete them.")]
		public async Task Execute([Summary("(role name)")] int timeout) {

			if (!IsAuthenticatedUser()) { // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;
			}

			if (timeout < 0) {
				await ReplyAsync($"Must give a value >= 0, got {timeout}.");
				await ReplyAsync($"Must give a value >= 0, got {timeout}.");
				return;
			}

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.MessageTimeout = timeout;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Message timeout updated to {timeout} seconds.");
		}
	}
}
