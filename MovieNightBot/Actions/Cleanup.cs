using Discord;
using Discord.Commands;

namespace MovieNightBot.Actions {
	public class Cleanup : AdminAction {

		[Command("cleanup")]
		[Summary("Removes all bot commands and messages in the current server message channel.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser() || !CheckForServerChannel()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;
			
			var messages = Context.Channel.GetMessagesAsync(2000);
			List<Discord.IMessage> messagesToDelete = new List<Discord.IMessage>();
			await foreach (var messageg in messages) {
				foreach (var message in messageg) {
					if (message.Author.Id == Context.Client.CurrentUser.Id || message.Content.StartsWith(Application.config.message_identifier))
						messagesToDelete.Add(message);
				}
			}

			//await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messagesToDelete);

			foreach (Discord.IMessage message in messagesToDelete)
				await message.DeleteAsync();

			return;
		}

	}
}
