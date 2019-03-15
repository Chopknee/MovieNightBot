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

    public class HelloWorld : ModuleBase<SocketCommandContext> {

        [Command("hello"), Alias("helloworld", "world"), Summary("Hello world command")]
        public async Task SayHello() {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == MoviesData.Model.GetAdminRoleName(Context.Guild));

                if (user.Roles.Contains(role)) {
                    await Context.Channel.SendMessageAsync($"Hi {Context.User.Username}. You are a co-captain.");
                } else {
                    await Context.Channel.SendMessageAsync($"Hi {Context.User.Username}. You are not co-captain, don't use this command again.");
                }
            } catch (DataException ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                await Context.Channel.SendMessageAsync("Your server is not in my database, please have a user with the role of 'Movie Master' run the initialize command! :flushed:");
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }
    }
}
