using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;
using MovieNightBot.Core.Data;
using System.Globalization;

namespace MovieNightBot.Core.Commands {
    //Gettin dat info on da movies
    class Info : ModuleBase<SocketCommandContext> {

        [Command("listwatched"), Summary("Show all watched movies.")]
        public async Task ListWatched() {
            Movie[] serverMovies = Movies.GetWatchedMovies(Context.Guild.Id + "", Context.Guild.Name);
            string mess = $"On the server {Context.Guild.Name}, they have watched the following;";
            foreach (Movie m in serverMovies) {
                mess += $"\n{m.Title} on {m.watchedDate}";
            }
            await Context.Channel.SendMessageAsync(mess);
        }

        [Command("listsuggested"), Summary("Show all suggested movies.")]
        public async Task ListSuggested() {
            Movie[] serverMovies = Movies.GetWaitingMovies(Context.Guild.Id + "", Context.Guild.Name);
            string mess = $"On the server {Context.Guild.Name}, they have suggested the following;";
            foreach (Movie m in serverMovies) {
                mess += $"\n{m.Title} on {m.watchedDate}";
            }
            await Context.Channel.SendMessageAsync(mess);
        }
    }
}
