using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MovieNightBot.Core.Commands {
    public class HelloWorld : ModuleBase<SocketCommandContext> {
        [Command("hello"), Alias("helloworld", "world"), Summary("Hello world command")]
        public async Task SayHello() {
            //Context.Guild.Roles.GetEnumerator
            //IReadOnlyCollection <Discord.WebSocket.SocketRole> userRoles = Context.Guild.GetUser(Context.User.Id).Roles;
            //Context.Guild.GetUser(Context.User.Id).

            //foreach (Discord.WebSocket.SocketRole role in userRoles) {
            //    await Context.Channel.SendMessageAsync($"{role.Id}");
            //}
            SocketGuildUser user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Movie Master");
            
            if (user.Roles.Contains(role)) {
                await Context.Channel.SendMessageAsync($"Hi {Context.User.Username}. You are a co-captain.");
            } else {
                await Context.Channel.SendMessageAsync($"Hi {Context.User.Username}. You are not co-captain, don't use this command again.");
            }
        }
    }
}
