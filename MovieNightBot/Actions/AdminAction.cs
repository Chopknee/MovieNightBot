using Discord.Commands;

namespace MovieNightBot.Actions {
	public class AdminAction : ModuleBase<SocketCommandContext> {
		public bool IsAuthenticatedUser() {
			Discord.WebSocket.SocketGuildUser guildUser = Context.Guild.GetUser(Context.User.Id);
			Database.Models.Server server = Database.Controller.Server.GetByGuildId(Context.Guild.Id);

			bool found = false;
			foreach (Discord.WebSocket.SocketRole role in guildUser.Roles) {
				if (role.Name == server.AdminRole) {
					found = true;
					break;
				}
			}

			return guildUser.GuildPermissions.Administrator || found;
		}
	}
}
