using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Commands {
    class TieOption : ModuleBase<SocketCommandContext> {
        public volatile static string[] CommandNames = new string[] { "help", "suggest", "watched" };

        [Command("tieoption"), Summary("Set how the bot handles ties.")]
        public async Task SetTieOption([Remainder]string Input = "") {//Because we could all use some from time to time.
            SocketGuildUser user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == ServerData.ADMIN_ROLE_NAME);
            int option = 0;
            switch (Input) {
                case "breaker":
                    option = 0;
                    break;
                case "random":
                    option = 1;
                    break;
                default:
                    await Context.User.SendMessageAsync("Unknown argument. For **m!tieoption** the options are *breaker* and *random*. Use **m!help** for more detials.");
                    return;
            }
            if (user.Roles.Contains(role)) {
                //This user is allowed to configure settings.
                ServerData.SetTiebreakerOption(Context.Guild.Id + "", Context.Guild.Name, option);
                await Context.User.SendMessageAsync($"MovieNightBot will now use the {Input} method for ties.");
            }
        }
    }
}
