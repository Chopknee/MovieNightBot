using Discord.Commands;

namespace MovieNightBot.Actions {
	public class BaseAction : ModuleBase<SocketCommandContext> {

		public static string genericErrorMessage = 
			"OOPSIE WOOPSIE!! UwU We made a fucky wucky!! A wittle fucko boingo! The code "
			+ "monkeys at our headquarters are working VEWY HAWD to fix this!";

		public bool CheckForServerChannel() {
			return Database.Controller.Server.GetByGuildId(Context.Guild.Id).ChannelId == Context.Channel.Id;
		}
	}
}
