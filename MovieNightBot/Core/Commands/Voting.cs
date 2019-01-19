using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;
using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Commands {
    public class Voting : ModuleBase<SocketCommandContext> {
        //We aren't worried about saving the current vote so this is volatile.
        public volatile static Dictionary<string, Movie[]> moveVoteOptions = new Dictionary<string, Movie[]>();
        //YEAH, a dictionary in a dictionary
        public volatile static Dictionary<string, Dictionary<string, int>> currentVotes = new Dictionary<string, Dictionary<string, int>>();

        [Command("beginvote"), Summary("Start the voging process for a movie.")]
        public async Task BeginVote() {
            await Context.Channel.SendMessageAsync($"Vote for these titles;");
            Movie[] movs = Movies.GetMovieVote("" + Context.Guild.Id, Context.Guild.Name);
            Console.WriteLine(movs.Length);
            moveVoteOptions["" + Context.Guild.Id] = movs;
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("What movie will we watch tonight?")
                .WithDescription($"use m!vote 0 - {movs.Length} to add your vote! You may only vote once!")
                .WithColor(new Color(0xE314C7))
                .WithTimestamp(DateTime.Now)
                .WithAuthor(author => {
                    author
                    .WithName("Movie Night Bot");
                });
            for (int i = 0; i < movs.Length; i++) {
                builder.AddField(i + "", $"{movs[i].Title} suggested on {movs[i].suggestDate}");
            }
            Embed embed = builder.Build();
            await Context.Channel.SendMessageAsync("Movie Vote!!!", embed: embed).ConfigureAwait(false);
        }

        [Command("showvote"), Summary("Ends the voting process and shows the final result.")]
        public async Task ShowVote() {
        }

        [Command("vote"), Summary("Vote for one of the movies listed.")]
        public async Task Vote([Remainder]string Input = "") {
            int vote = -1;
            //Input sanitization and filtering
            
            if (Input.Equals("")) return;
            if (!int.TryParse(Input, out vote)) return;
            if (vote == -1) return;
            if (vote < 1 || vote > moveVoteOptions[Context.Guild.Id + ""].Length) {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, your vote was outside the valid range.");
                return;
            }
            Dictionary<string, int> serverVotes = currentVotes[Context.Guild.Id + ""];
            //If the votes dictionary for this server does not exist, create it.
            if (serverVotes == null) { serverVotes = new Dictionary<string, int>(); currentVotes.Add(Context.Guild.Id + "", serverVotes); }
            //
            if (serverVotes.ContainsKey(Context.User.Id + "")) {
                //Update the user vote
                serverVotes[Context.User.Id + ""] = vote;
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, your vote has been updated to {vote}");
            } else {
                serverVotes[Context.User.Id + ""] = vote;
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, you have cast your vote for option {vote}");
            }
        }
    }

    public struct Vote {
        public string UUID;
        public int vote;
    }
}
