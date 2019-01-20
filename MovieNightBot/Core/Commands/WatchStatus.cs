using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;
using MovieNightBot.Core.Data;
using System.Globalization;

namespace MovieNightBot.Core.Commands {
    //Setting / unsetting watched
    public class WatchStatus : ModuleBase<SocketCommandContext> {

        [Command("setwatched"), Summary("Sets a movie as having been watched. That movie will no longer show up in votes.")]
        public async Task SetAsWatched([Remainder]string Input = "") {
            //Input sanitization
            if (Input.Equals("")) { Console.WriteLine("A suggestion was made with no text."); return;  }; //Filter out empty suggestions
            if (Input.Length > 150) { Console.WriteLine("A movie title was suggested that was over 150 characters."); return; }//Title too long, probably spam
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            Input = Input.Trim();//Clear spaces
            Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
            Console.WriteLine(Input);
            //Check if the movie has been suggested
            if (!Movies.HasMovieBeenSuggested(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been suggested yet.");
                return;
            }
            Movies.SetMovieToWatched(Context.Guild.Id + "", Context.Guild.Name, Input);
            await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} is now set to watched and will no longer appear on votes.\nTo undo this, you can use **m!unwatch {Input}**.");
        }

        [Command("unwatch"), Summary("Returns a previously watched movie to the voting lists.")]
        public async Task SetUnwatched([Remainder]string Input = "") {
            //Input sanitization
            if (Input.Equals("")) { Console.WriteLine("A suggestion was made with no text."); return;  }; //Filter out empty suggestions
            if (Input.Length > 150) { Console.WriteLine("A movie title was suggested that was over 150 characters."); return; }//Title too long, probably spam
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            Input = Input.Trim();//Clear spaces
            Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
            //Check if the movie has been suggested
            if (!Movies.HasMovieBeenWatched(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been watched yet.");
                return;
            }
            Movies.SetMovieToUnwatched(Context.Guild.Id + "", Context.Guild.Name, Input);
            await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has been added back to the wait list and will show in future votes.\nTo undo this, you can use **m!setwatched {Input}**.");
        }
    }
}