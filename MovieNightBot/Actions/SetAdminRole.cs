using Discord.Commands;

namespace MovieNightBot.Actions {
	public class SetAdminRole : AdminAction {

		[Command("set_admin_role")]
		[Summary("Sets the name of the role that is allowed to run admin only commands in movie night bot.\nServer administrators have full privileges by default")]
		[RequireContext(ContextType.Guild)]
		public async Task Execute() {

			if (!IsAuthenticatedUser()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			await ReplyAsync(
				$"Server admin role is {Database.Controller.Server.GetByGuildId(Context.Guild.Id).AdminRole}."
				+ " If you wish to chage it use set_admin_role and specify a new value.");
		}

		[Command("set_admin_role")]
		[Summary("Sets the name of the role that is allowed to run admin only commands in movie night bot.\nServer administrators have full privileges by default")]
		public async Task Execute([Remainder][Summary("(role name)")] string roleName) {

			if (!IsAuthenticatedUser() || !CheckForServerChannel()) // For non-authenticated users, just return. No need to respond in order to prevent spam.
				return;

			if (roleName == null || roleName == string.Empty) {
				await ReplyAsync("Must give a value for set_admin_role command");
				return;
			}

			using (var controller = Database.Controller.GetDBController()) {
				Database.Models.Server server = controller.Servers.Single(server => server.Id == Context.Guild.Id);
				server.AdminRole = roleName;
				await controller.SaveChangesAsync();
			}

			await ReplyAsync($"Server admin role name is now {roleName}.");
		}
	}
}
