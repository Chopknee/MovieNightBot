using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;
using MovieNightBot.Core.Data;
using System.Globalization;

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
            if (Input.Length > 150) return;//Title too long, probably spam
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            Input = Input.Trim();//Clear spaces
            Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case

            if (!MoviesData.Model.HasMovie(Context.Guild, Input)) {
                MoviesData.Model.SuggestMovie(Context.Guild, Input);
                await Context.Channel.SendMessageAsync($"Your suggestion of {Input} has been added to the list.");
                return;
            }
            await Context.Channel.SendMessageAsync($"Your suggestion of {Input} is either already on the list, or has been watched already.");
        }
    }
}
