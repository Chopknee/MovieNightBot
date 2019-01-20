using System.Threading.Tasks;
using MovieNightBot.Core.Data;
using Discord.Commands;

namespace MovieNightBot.Core.Commands {
    public class MoviesInfo : ModuleBase<SocketCommandContext> {
        [Command("watched"), Summary("Hello world command")]
        public async Task ListWatchedMovies() {
            Movie[] serverMovies = Movies.GetWatchedMovies(Context.Guild.Id + "", Context.Guild.Name);
            string mess = $"On the server {Context.Guild.Name}, they have watched the following;";
            foreach (Movie m in serverMovies) {
                mess += $"\n**{m.Title}** watched on {m.watchedDate}";
            }
            await Context.Channel.SendMessageAsync(mess);
        }

        [Command("suggested"), Summary("Show all suggested movies.")]
        public async Task ListSuggestedMovies() {
            Movie[] serverMovies = Movies.GetWaitingMovies(Context.Guild.Id + "", Context.Guild.Name);
            string mess = $"On the server {Context.Guild.Name}, the following have been suggested;";
            foreach (Movie m in serverMovies) {
                mess += $"\n**{m.Title}**";
            }
            await Context.Channel.SendMessageAsync(mess);
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Text;

//using Discord;
//using Discord.Commands;
//using MovieNightBot.Core.Data;
//using System.Globalization;

//namespace MovieNightBot.Core.Commands {
//    //Gettin dat info on da movies
//    class Info : ModuleBase<SocketCommandContext> {
//        //THESE are not being recognized as commands .
//        // I don't know why, they were copied and pased directly from the example "hello world".
//        [Command("listwatched"), Summary("Show all watched movies.")]
//        public async Task ListWatched() {
//            Console.WriteLine("H TO THE EFFING ELLO");

//            Movie[] serverMovies = Movies.GetWatchedMovies(Context.Guild.Id + "", Context.Guild.Name);
//            string mess = $"On the server {Context.Guild.Name}, they have watched the following;";
//            foreach (Movie m in serverMovies) {
//                mess += $"\n{m.Title} on {m.watchedDate}";
//            }
//            await Context.Channel.SendMessageAsync(mess);
//        }

//        [Command("listsuggested"), Summary("Show all suggested movies.")]
//        public async Task ListSuggested() {
//            Console.WriteLine("H TO THE EFFING ELLO");
//            Movie[] serverMovies = Movies.GetWaitingMovies(Context.Guild.Id + "", Context.Guild.Name);
//            string mess = $"On the server {Context.Guild.Name}, they have suggested the following;";
//            foreach (Movie m in serverMovies) {
//                mess += $"\n{m.Title} on {m.watchedDate}";
//            }
//            await Context.Channel.SendMessageAsync(mess);
//        }
//    }
//}
