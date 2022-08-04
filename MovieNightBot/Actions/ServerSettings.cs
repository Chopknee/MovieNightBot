using Discord;
using Discord.Commands;
using System.Reflection;

namespace MovieNightBot.Actions {
	public class ServerSettings : AdminAction {

		[Command("server_settings")]
		[Summary("View the current server settings.")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {
			try {
				if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
					return;

				Database.Models.Server server = Database.Controller.Server.GetByGuildId(Context.Guild.Id);

				if (server == null) {
					await ReplyAsync(genericErrorMessage);
				}
					

				string[] ignoreAttrs = { "Id", "Movies" };

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

					if (prop.GetValue(server, null) == null)
						continue;

					embed.AddField(prop.Name, prop.GetValue(server, null));
				}

				Console.WriteLine("SERVER SETTINGS");

				await ReplyAsync(embed: embed.Build());
			} catch (Exception ex) {
				Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
			}
		}
	}
}
