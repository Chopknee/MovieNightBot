using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Commands {

    //MAKE SURE THE CLASS IS PUBLIC!!!
    public class MovieVoteCount : ModuleBase<SocketCommandContext> {

        /**
         * SetMovieCount
         */
        [Command("movie_vote_count")]
        [Summary("Sets the number of movies that will be selected for a vote.")]
        public async Task SetMovieCount([Summary ("The number of movies to be selected.")] int number = 5) {
            SocketGuildUser user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == MoviesData.Model.GetAdminRoleName(Context.Guild));

            if (user.Roles.Contains(role)) {
                try {
                    //int count = -1;
                    //Input sanitization and filtering
                    if (number.Equals("")) { Console.WriteLine("Empty argument."); return; }//Empty argument

                    if (number < 2) {//Valid range check
                        Console.WriteLine("Can't be less than 2.");
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, vote count can't be less than 2.");
                        return;
                    }

                    //Set the limit in the server file.
                    MoviesData.Model.SetMovieVoteCount(Context.Guild, number);
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, vote count has been set to {number}.");
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
            } else {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, you need to have the role {MoviesData.Model.GetAdminRoleName(Context.Guild)} to use this command.");
            }
        }
    }
}
