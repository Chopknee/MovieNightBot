using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;
using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Commands {
    public class SuggestMovie : ModuleBase<SocketCommandContext> {
        [Command("suggest"), Summary("Add movie command")]
        public async Task Suggest([Remainder]string Input = "None") {
            //So yeah, get the file for this server, check if the movie was suggested, then add it!
            string fileName = Context.Guild.Name + Context.Guild.Id + ".txt";//Filename used for channel
            if (!Movies.HasMovie(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                Movies.SuggestMovie(Context.Guild.Id + "", Context.Guild.Name, Input);
                await Context.Channel.SendMessageAsync($"Your suggestion of {Input} has been added to the list.");
                return;
            }
            await Context.Channel.SendMessageAsync($"Your suggestion of {Input} is either already on the list, or has been watched already.");
        }
    }
}
