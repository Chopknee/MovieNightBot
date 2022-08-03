using Discord;
using Discord.Commands;
using System.Reflection;

namespace MovieNightBot.Actions {
	public class ServerSettings : AdminAction {

		[Command("server_settings")]
		[Summary("View the current server settings.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			Database.Models.Server server = Database.Controller.Server.GetByGuildId(Context.Guild.Id);

			string[] ignoreAttrs = { "id" };

			//generate an embed
			var embed = new EmbedBuilder {
				Title = $"{Context.Guild.Name} Settings",
				Description = $"Current settings for server '{Context.Guild.Name}'"
			};

			foreach (PropertyInfo prop in typeof(Database.Models.Server).GetProperties()) {
				bool bSkip = false;
				foreach (string ignoreAttr in ignoreAttrs) {
					if (ignoreAttr == prop.Name) {
						bSkip = true;
						break;
					}
				}
				if (bSkip)
					continue;
				//Add the property to the embed
				embed.AddField(prop.Name, prop.GetValue(server, null));
			}

			await ReplyAsync(embed: embed.Build());
		}
	}
}
