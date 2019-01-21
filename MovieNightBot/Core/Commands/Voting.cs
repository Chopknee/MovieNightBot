using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Commands {
    public class Voting : ModuleBase<SocketCommandContext> {
        //We aren't worried about saving the current vote so this is volatile.
        public volatile static Dictionary<string, Movie[]> movieVoteOptions = new Dictionary<string, Movie[]>();
        //YEAH, a dictionary in a dictionary
        public volatile static Dictionary<string, Dictionary<string, int>> currentVotes = new Dictionary<string, Dictionary<string, int>>();

        [Command("beginvote"), Summary("Start the voging process for a movie.")]
        public async Task BeginVote() {
            if (movieVoteOptions.ContainsKey("" + Context.Guild.Id)) {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, a vote has already been started. If you wish to end the current vote, use **m!showvote**.");
                return;
            }

            await Context.Channel.SendMessageAsync("Vote for these titles;");
            Movie[] movs = ServerData.GetMovieVote("" + Context.Guild.Id, Context.Guild.Name);
            Console.WriteLine(movs.Length);
            movieVoteOptions["" + Context.Guild.Id] = movs;
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
                builder.AddField($"{(i+1)}] {movs[i].Title}", $"**m!vote** {(i+1)}");
            }
            Embed embed = builder.Build();
            currentVotes.Add("" + Context.Guild.Id, new Dictionary<string, int>());
            await Context.Channel.SendMessageAsync("Movie Vote!!!", embed: embed).ConfigureAwait(false);
        }

        [Command("showvote"), Summary("Ends the voting process and shows the final result.")]
        public async Task ShowVote() {
            try {
                if (!movieVoteOptions.ContainsKey("" + Context.Guild.Id)) {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, no vote has been started. If you wish to start a vote please use **m!beginvote**.");
                    return;
                }
                if (!currentVotes.ContainsKey(Context.Guild.Id + "")) {
                    await Context.Channel.SendMessageAsync($"Apologies, an unexpected error has prevented the vote from completing successfully. Please restart the vote by using **m!beginvote**.");
                }
                Movie[] movies = movieVoteOptions["" + Context.Guild.Id];
                //Build an array to count the votes
                int[] votes = new int[movies.Length];
                //This ends the vote, then tallies up the numbers.
                Dictionary<string, int> serverVotes = currentVotes[Context.Guild.Id + ""];
                foreach (KeyValuePair<string, int> entry in serverVotes) {
                    votes[entry.Value] += 1;//Tally up the votes
                }
                //We know what the max vote count is.
                int max = 0;
                foreach (int option in votes) {
                    max = Math.Max(option, max);
                }
                if (max == 0) {
                    await Context.Channel.SendMessageAsync($"The voting was ended before anyone cast a vote. To restart voting, use **m!beginvote**.");
                    movieVoteOptions.Remove("" + Context.Guild.Id);
                    currentVotes.Remove("" + Context.Guild.Id);
                    return;
                }
                //Determining winning votes
                List<int> winners = new List<int>();
                for (int i = 0; i < votes.Length; i++) {
                    if (votes[i] == max) {
                        winners.Add(i);
                    }
                }
                Console.WriteLine(winners[0]);
                if (winners.Count == 1) {
                    //We have a single winner
                    await Context.Channel.SendMessageAsync($"The winner of this movie night vote is {movies[winners[0]].Title}.\nTo remove it from future votes, use **m!setwatched {movies[winners[0]].Title}**");
                    //The movie needs to be added to the already watched list, then the vote system needs to be reset.
                    movieVoteOptions.Remove("" + Context.Guild.Id);
                    currentVotes.Remove("" + Context.Guild.Id);
                    return;
                } else if (winners.Count > 1) {
                    //There was a more than 1 way tie
                    await Context.Channel.SendMessageAsync("There was a tie between two or more movies. Generating new vote.");
                    foreach (int tie in winners) {
                        await Context.Channel.SendMessageAsync($"{movies[tie].Title}");
                    }
                    movieVoteOptions.Remove("" + Context.Guild.Id);
                    currentVotes.Remove("" + Context.Guild.Id);
                    //There are two routes I could take
                    //  1. Restart the vote with the tied options
                    //  2. Generate a totally new vote.
                    //      For now this is option 2.
                    await BeginVote();
                    return;
                } else {
                    //There is some kind of problem with my math
                    await Context.Channel.SendMessageAsync("Apologies, some kind of counting error has happened. No winning vote was determined. Please restart the vote using m!beginvote.");
                    movieVoteOptions.Remove("" + Context.Guild.Id);
                    currentVotes.Remove("" + Context.Guild.Id);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        [Command("vote"), Summary("Vote for one of the movies listed.")]
        public async Task Vote([Remainder]string Input = "") {
            try {
                int vote = -1;
                //Input sanitization and filtering
                if (!currentVotes.ContainsKey("" + Context.Guild.Id)) { Console.WriteLine("No vote has been started."); return; } //No vote started
                if (Input.Equals("")) { Console.WriteLine("Empty argument."); return; }//Empty argument
                if (!int.TryParse(Input, out vote)) { Console.WriteLine("Invalid vote number."); return; }//Convert to integer/filter out non integer input
                if (vote < 1 || vote > movieVoteOptions[Context.Guild.Id + ""].Length) {//Valid voting range check
                    Console.WriteLine("Invalid voting range.");
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, your vote was outside the valid range.");
                    return;
                }
                vote = vote - 1;//Adjust the vote to the correct range.
                Dictionary<string, int> serverVotes = currentVotes[Context.Guild.Id + ""];
                //If the votes dictionary for this server does not exist, create it.
                if (serverVotes == null) { serverVotes = new Dictionary<string, int>(); currentVotes.Add(Context.Guild.Id + "", serverVotes); }
                    
                if (serverVotes.ContainsKey(Context.User.Id + "")) {
                    if (serverVotes[Context.User.Id + ""] == vote) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, your vote was already cast for option {vote + 1}.");
                    } else {
                        //Update the user vote
                        serverVotes[Context.User.Id + ""] = vote;
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, your vote has been updated to option {vote + 1}.");
                    }
                } else {
                    serverVotes[Context.User.Id + ""] = vote;
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, you have cast your vote for option {vote + 1}.");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        [Command("moviecount"), Summary("Vote for one of the movies listed.")]
        public async Task SetMovieCount([Remainder]string Input = "") {
            SocketGuildUser user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == ServerData.ROLE_NAME);

            if (user.Roles.Contains(role)) {
                try {
                    int count = -1;
                    //Input sanitization and filtering
                    if (Input.Equals("")) { Console.WriteLine("Empty argument."); return; }//Empty argument
                    if (!int.TryParse(Input, out count)) { Console.WriteLine("Not a number."); return; }//Convert to integer/filter out non integer input
                    if (count < 2) {//Valid range check
                        Console.WriteLine("Can't be less than 2.");
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, vote count can't be less than 2.");
                        return;
                    }

                    //Set the limit in the server file.
                    ServerData.SetMovieVoteCount(Context.Guild.Id + "", Context.Guild.Name, count);
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, vote count has been set to {count}.");
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
            } else {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, you need to have the role {ServerData.ROLE_NAME} to use this command.");
            }
        }

    }
}
