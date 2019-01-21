using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using MovieNightBot.Core.Data;



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
            if (!ServerData.HasMovieBeenSuggested(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been suggested yet.");
                return;
            }
            ServerData.SetMovieToWatched(Context.Guild.Id + "", Context.Guild.Name, Input);
            await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} is now set to watched and will no longer appear on votes.\nTo undo this, you can use **m!unwatch {Input}**.");
        }

        [Command("unwatch"), Summary("Returns a previously watched movie to the voting lists.")]
        public async Task SetUnwatched([Remainder]string Input = "") {
            //Input sanitization
            if (Input.Equals("")) { Console.WriteLine("This command requires a movie name."); return;  }; //Filter out empty suggestions
            if (Input.Length > 150) { Console.WriteLine("Movie title too long."); return; }//Title too long, probably spam
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            Input = Input.Trim();//Clear spaces
            Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
            //Check if the movie has been suggested
            if (!ServerData.HasMovieBeenWatched(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been watched yet.");
                return;
            }
            ServerData.SetMovieToUnwatched(Context.Guild.Id + "", Context.Guild.Name, Input);
            await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has been added back to the wait list and will show in future votes.\nTo undo this, you can use **m!setwatched {Input}**.");
        }

        [Command("remove"), Summary("Returns a previously watched movie to the voting lists.")]
        public async Task RemoveMovie([Remainder]string Input = "") {
            SocketGuildUser user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == ServerData.ROLE_NAME);

            if (user.Roles.Contains(role)) {
                //Input sanitization
                if (Input.Equals("")) { Console.WriteLine("This command requires a movie name."); return; }; //Filter out empty suggestions
                if (Input.Length > 150) { Console.WriteLine("Movie title too long."); return; }//Title too long, probably spam
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                Input = Input.Trim();//Clear spaces
                Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
                                                //Check if the movie has been suggested
                if (!ServerData.HasMovieBeenSuggested(Context.Guild.Id + "", Context.Guild.Name, Input)) {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been watched yet.");
                    return;
                }
                if (ServerData.RemoveMovie(Context.Guild.Id + "", Context.Guild.Name, Input))
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has been removed.");
            } else {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, you need to have the role {ServerData.ROLE_NAME} to use this command.");
            }
        }
    }
}