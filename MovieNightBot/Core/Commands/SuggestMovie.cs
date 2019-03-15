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
            try {
                if (Input.Equals("")) return; //Filter out empty suggestions
                if (Input.Length > 150) return;//Title too long, probably spam
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                Input = Input.Trim();//Clear spaces
                Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case

                if (MoviesData.Model.MovieHasBeenSuggested(Context.Guild, Input)) {
                    await Context.Channel.SendMessageAsync($"The movie {Input} has already been suggested.");
                    return;
                } else if (MoviesData.Model.MovieHasBeenWatched(Context.Guild, Input)) {
                    await Context.Channel.SendMessageAsync($"The movie {Input} has already been watched.");
                    return;
                }
                MoviesData.Model.SuggestMovie(Context.Guild, Input);
                await Context.Channel.SendMessageAsync($"Your suggestion of {Input} has been added to the list.");
                return;
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
