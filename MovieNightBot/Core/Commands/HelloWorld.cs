using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;

namespace MovieNightBot.Core.Commands {
    public class HelloWorld : ModuleBase<SocketCommandContext> {
        [Command("hello"), Alias("helloworld", "world"), Summary("Hello world command")]
        public async Task SayHello() {
            await Context.Channel.SendMessageAsync("Hello world");
        }
    }
}
