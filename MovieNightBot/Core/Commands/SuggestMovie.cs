using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;
using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Commands {
    public class SuggestMovie : ModuleBase<SocketCommandContext> {
        /**
         * Users can suggest movies. [command] [movie name]
         * 
         * In the future, this will filter out attempts at adding movies twice.
         * -Look for different versions using caps/no caps
         * -Filter extra spaces
         * -(Maybe) Filter l33t sp3ak and other attempts to duplicate movies
         */
        [Command("suggest"), Summary("Add movie command")]
        public async Task Suggest([Remainder]string Input = "") {
            if (Input.Equals(""))  return; //Filter out empty suggestions
            string fileName = Context.Guild.Name + Context.Guild.Id + ".txt";
            if (!Movies.HasMovie(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                Movies.SuggestMovie(Context.Guild.Id + "", Context.Guild.Name, Input);
                await Context.Channel.SendMessageAsync($"Your suggestion of {Input} has been added to the list.");
                return;
            }
            await Context.Channel.SendMessageAsync($"Your suggestion of {Input} is either already on the list, or has been watched already.");
        }
    }
}
