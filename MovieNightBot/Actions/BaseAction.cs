using Discord.Commands;

namespace MovieNightBot.Actions {
	public class BaseAction : ModuleBase<SocketCommandContext> {

		public bool CheckForServerChannel() {
			return Database.Controller.Server.GetByGuildId(Context.Guild.Id).ChannelId == Context.Channel.Id;
		}
	}
}
