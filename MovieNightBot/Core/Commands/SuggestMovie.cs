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
                if (Input.Equals("") || Input.Length > 150) return; //Filter out bad input
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                Input = Input.Trim();//Clear spaces
                Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
                ServerData sd = ServerData.Get(Context.Guild);
                if (sd.DrunkoModeEnabled == true) {
                    await Context.Channel.SendMessageAsync("Hey, suggestions are disabled right now.");
                    return;
                }
                Movie m = sd.GetMovie(Input);
                if (m != null) {
                    if (m.Watched) {
                        await Context.Channel.SendMessageAsync($"The movie {Input} has already been watched.");
                        return;
                    } else {
                        await Context.Channel.SendMessageAsync($"The movie {Input} has already been suggested.");
                        return;
                    }
                }
                sd.AddMovie(Input, Context.User.Username);
                await Context.Channel.SendMessageAsync($"Your suggestion of {Input} has been added to the list.");
                Program.Instance.OnMoviesListModified?.Invoke(m, Context.Guild, Context.Channel, Context.User);
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
