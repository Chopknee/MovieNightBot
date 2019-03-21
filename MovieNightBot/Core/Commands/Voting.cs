using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MovieNightBot.Core.Data;

using Discord.Rest;

namespace MovieNightBot.Core.Commands {

    //public class Voting:ModuleBase<SocketCommandContext> {

    //    //We aren't worried about saving the current vote so this is volatile.
    //    public volatile static Dictionary<string,Movie[]> movieVoteOptions = new Dictionary<string,Movie[]> ();
    //    //YEAH, a dictionary in a dictionary
    //    public volatile static Dictionary<string,Dictionary<string,int>> currentVotes = new Dictionary<string,Dictionary<string,int>> ();
    //    //Stores the embed message generated for the vote.
    //    public volatile static Dictionary<string,RestUserMessage> VoteMessage = new Dictionary<string,RestUserMessage> ();

    //    public volatile static Dictionary<string, Dictionary<string, Voter>> serversAndVoters = new Dictionary<string, Dictionary<string, Voter>>();


    //    [Command ( "begin_vote" ), Summary ( "Start the voging process for a movie." )]
    //    [RequireBotPermission ( GuildPermission.AddReactions )]
    //    public async Task BeginVote () {
    //        //Checking for a vote that has already started.
    //        if (movieVoteOptions.ContainsKey ( "" + Context.Guild.Id )) {
    //            await Context.Channel.SendMessageAsync ( $"{Context.User.Username}, a vote has already been started. If you wish to end the current vote, use **m!showvote**." );
    //            return;
    //        }

    //        //Getting random list of movies to vote on.
    //        Movie[] movs;
    //        try {
    //            movs = MoviesData.Model.GetRandomVote ( Context.Guild );
    //            movieVoteOptions.Add ( "" + Context.Guild.Id,movs );
    //        } catch (DataException ex) {
    //            Console.WriteLine ( ex.Message + "\n" + ex.StackTrace );
    //            await Context.Channel.SendMessageAsync ( "I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:" );
    //            return;
    //        }

    //        //Build the embed, then send it out
    //        try {
    //            Embed embed = MakeVoteEmbed ( movs,null );
    //            currentVotes.Add ( "" + Context.Guild.Id,new Dictionary<string,int> () );
    //            RestUserMessage mess = await Context.Channel.SendMessageAsync ( "Movie Vote!!!",embed: embed ).ConfigureAwait ( false );
    //            VoteMessage.Add ( Context.Guild.Id.ToString (),mess );
    //            Emoji[] votes = new Emoji[movs.Length];
    //            for (int i = 0;i < movs.Length;i++) {
    //                votes[i] = MojiCommand.VoteMoji[i];
    //            }
    //            //votes[votes.Length] = MojiCommand.CommandMoji[0];
    //            await mess.AddReactionsAsync ( votes );
    //        } catch (Exception ex) {
    //            Console.WriteLine ( ex.Message + "\n" + ex.StackTrace );
    //        }
    //    }

    //    [Command ( "make_vote" ), Summary ( "Start a vote from a custom list of things. Separate different options with a semicolon. ;" )]
    //    public async Task MakeVote ( [Remainder] string Input = "" ) {
    //        if (Input == "") {
    //            await Context.Channel.SendMessageAsync ( "I can't start a vote on nothing." );
    //            return;
    //        }
    //        string[] things = Input.Split ( ";" );
    //        if (things.Length < 2) {
    //            await Context.Channel.SendMessageAsync ( "I can't start a vote on only one item." );
    //            return;
    //        }
    //        Movie[] movs = new Movie[things.Length];
    //        for (int i = 0;i < movs.Length;i++) {
    //            movs[i] = new Movie {
    //                Title = things[i]
    //            };
    //        }
    //        movieVoteOptions.Add ( "" + Context.Guild.Id,movs );
    //        try {
    //            Embed embed = MakeVoteEmbed ( movs,null );
    //            currentVotes.Add ( "" + Context.Guild.Id,new Dictionary<string,int> () );
    //            RestUserMessage mess = await Context.Channel.SendMessageAsync ( "Movie Vote!!!",embed: embed ).ConfigureAwait ( false );
    //            VoteMessage.Add ( Context.Guild.Id.ToString (),mess );
    //            for (int i = 0;i < movs.Length;i++) {
    //                await mess.AddReactionAsync (MojiCommand.VoteMoji[i] );
    //            }
    //        } catch (Exception ex) {
    //            Console.WriteLine ( ex.Message + "\n" + ex.StackTrace );
    //        }
    //    }

    //    [Command ( "show_vote" ), Summary ( "Ends the voting process and shows the final result." )]
    //    public async Task ShowVote () {
    //        try {
    //            if (!movieVoteOptions.ContainsKey ( Context.Guild.Id.ToString () )) {
    //                await Context.Channel.SendMessageAsync ( $"{Context.User.Username}, no vote has been started. If you wish to start a vote please use **m!beginvote**." );
    //                return;
    //            }
    //            if (!currentVotes.ContainsKey ( Context.Guild.Id.ToString () )) {
    //                await Context.Channel.SendMessageAsync ( $"Apologies, an unexpected error has prevented the vote from completing successfully. Please restart the vote by using **m!beginvote**." );
    //            }
    //            Movie[] movies = movieVoteOptions["" + Context.Guild.Id];
    //            //Build an array to count the votes
    //            int[] votes = new int[movies.Length];
    //            //This ends the vote, then tallies up the numbers.
    //            Dictionary<string,int> serverVotes = currentVotes[Context.Guild.Id.ToString ()];
    //            foreach (KeyValuePair<string,int> entry in serverVotes) {
    //                votes[entry.Value] += 1;//Tally up the votes
    //            }
    //            //We know what the max vote count is.
    //            int max = 0;
    //            foreach (int option in votes) {
    //                max = Math.Max ( option,max );
    //            }
    //            if (max == 0) {
    //                await Context.Channel.SendMessageAsync ( $"The voting was ended before anyone cast a vote. To restart voting, use **m!beginvote**." );
    //                movieVoteOptions.Remove ( Context.Guild.Id.ToString () );
    //                currentVotes.Remove ( Context.Guild.Id.ToString () );
    //                return;
    //            }
    //            //Determining winning votes
    //            List<int> winners = new List<int> ();
    //            for (int i = 0;i < votes.Length;i++) {
    //                if (votes[i] == max) {
    //                    winners.Add ( i );
    //                }
    //            }
    //            Console.WriteLine ( winners[0] );
    //            if (winners.Count == 1) {
    //                //We have a single winner
    //                await Context.Channel.SendMessageAsync ( $"The winner of this movie night vote is {movies[winners[0]].Title}.\nTo remove it from future votes, use **m!setwatched {movies[winners[0]].Title}**" );
    //                //The movie needs to be added to the already watched list, then the vote system needs to be reset.
    //                movieVoteOptions.Remove ( Context.Guild.Id.ToString () );
    //                currentVotes.Remove ( Context.Guild.Id.ToString () );
    //                VoteMessage.Remove ( Context.Guild.Id.ToString () );
    //                return;
    //            } else if (winners.Count > 1) {
    //                //There was a more than 1 way tie
    //                await Context.Channel.SendMessageAsync ( "There was a tie between two or more movies. Generating new vote." );
    //                foreach (int tie in winners) {
    //                    await Context.Channel.SendMessageAsync ( $"{movies[tie].Title}" );
    //                }
    //                movieVoteOptions.Remove ( Context.Guild.Id.ToString () );
    //                currentVotes.Remove ( Context.Guild.Id.ToString () );
    //                VoteMessage.Remove ( Context.Guild.Id.ToString () );
    //                //There are two routes I could take
    //                //  1. Restart the vote with the tied options
    //                //  2. Generate a totally new vote.
    //                //      For now this is option 2.
    //                await BeginVote ();
    //                return;
    //            } else {
    //                //There is some kind of problem with my math
    //                await Context.Channel.SendMessageAsync ( "Apologies, some kind of counting error has happened. No winning vote was determined. Please restart the vote using m!beginvote." );
    //                movieVoteOptions.Remove ( "" + Context.Guild.Id );
    //                currentVotes.Remove ( "" + Context.Guild.Id );
    //                VoteMessage.Remove ( Context.Guild.Id.ToString () );
    //            }
    //        } catch (Exception ex) {
    //            Console.WriteLine ( ex.Message + "\n" + ex.StackTrace );
    //        }
    //    }

    //    [Command ( "vote" ), Summary ( "Vote for one of the movies listed." )]
    //    public async Task Vote ( [Remainder]string Input = "" ) {
    //        try {
    //            Movie[] movs = movieVoteOptions[Context.Guild.Id + ""];
    //            int vote = -1;
    //            //Input sanitization and filtering
    //            if (!currentVotes.ContainsKey ( "" + Context.Guild.Id )) { Console.WriteLine ( "No vote has been started." ); return; } //No vote started
    //            if (Input.Equals ( "" )) { Console.WriteLine ( "Empty argument." ); return; }//Empty argument
    //            if (!int.TryParse ( Input,out vote )) { Console.WriteLine ( "Invalid vote number." ); return; }//Convert to integer/filter out non integer input
    //            if (vote < 1 || vote > movieVoteOptions[Context.Guild.Id + ""].Length) {//Valid voting range check
    //                Console.WriteLine ( "Invalid voting range." );
    //                await Context.Channel.SendMessageAsync ( $"{Context.User.Username}, your vote was outside the valid range." );
    //                return;
    //            }
    //            vote = vote - 1;//Adjust the vote to the correct range.
    //            int res = PlaceVote ( vote,Context.Guild.Id,Context.User.Id );
    //            if (res == 0) {

    //            } else if (res == 1) {
    //                await Context.Channel.SendMessageAsync ( $"{Context.User.Username}, you voted for {movs[vote].Title}." );
    //            } else if (res == 2) {
    //                await Context.Channel.SendMessageAsync ( $"{Context.User.Username}, your vote has been updated to {movs[vote].Title}." );
    //            } else if (res == 3) {
    //                await Context.Channel.SendMessageAsync ( $"{Context.User.Username}, your vote was already cast for {movs[vote].Title}." );
    //            }
    //        } catch (Exception ex) {
    //            Console.WriteLine ( ex.Message + "\n" + ex.StackTrace );
    //        }
    //    }

    //    public static async Task Vote ( IUserMessage Message,ISocketMessageChannel channel,SocketReaction reaction ) {
    //        SocketTextChannel chan = channel as SocketTextChannel;
    //        if (VoteMessage.ContainsKey ( chan.Guild.Id.ToString () )) {
    //            if (VoteMessage[chan.Guild.Id.ToString ()].Id == Message.Id) {
    //                int voteOption = 0;
    //                //Basically, now we convert the option into an integer, then go from there.
    //                if ((voteOption = MojiCommand.EmojiToVoteNumber( reaction.Emote )) != -1) {
    //                    PlaceVote ( voteOption,chan.Guild.Id,reaction.UserId );
    //                    //Update the vote embed.
    //                    Dictionary<string,int> serverVotes = currentVotes[chan.Guild.Id + ""];

    //                    await VoteMessage[chan.Guild.Id.ToString ()].ModifyAsync ( VoteMessage => {
    //                        VoteMessage.Content = "Movie Vote!!!";
    //                        VoteMessage.Embed = MakeVoteEmbed ( movieVoteOptions[chan.Guild.Id + ""],serverVotes );
    //                    } );
    //                }
    //            }
    //        }
    //    }

    //        private static int PlaceVote ( int vote, ulong guildId, ulong userId ) {
    //        try {
    //            Dictionary<string,int> serverVotes = currentVotes[guildId + ""];
    //            //If the votes dictionary for this server does not exist, create it.
    //            if (serverVotes == null) { serverVotes = new Dictionary<string,int> (); currentVotes.Add ( guildId + "",serverVotes ); }

    //            if (serverVotes.ContainsKey ( guildId + "" )) {
    //                if (serverVotes[userId + ""] == vote) {
    //                    //Already voted
    //                    return 3;
    //                } else {
    //                    //Update the user vote
    //                    serverVotes[userId + ""] = vote;
    //                    return 2;
    //                }
    //            } else {
    //                //New vote
    //                serverVotes[userId + ""] = vote;
    //                return 1;
    //            }
    //        } catch (Exception ex) {
    //            Console.WriteLine ( ex.Message + "\n" + ex.StackTrace );
    //            return 0;
    //        }
    //    }

    //    private static Embed MakeVoteEmbed ( Movie[] movies,Dictionary<string,int> serverVotes ) {
    //        int[] votes = null;
    //        if (serverVotes != null) {
    //            votes = GetVoteTotals ( serverVotes,movies );
    //        }
    //        EmbedBuilder builder = new EmbedBuilder ()
    //            .WithTitle ( "What movie will we watch tonight?" )
    //            .WithDescription ( $"React with the emoji next to the movie you wish to vote for." )
    //            .WithColor ( new Color ( 0xE314C7 ) )
    //            .WithTimestamp ( DateTime.Now )
    //            .WithAuthor ( author => {
    //                author
    //                .WithName ( "Movie Night Bot" );
    //            } );
    //        for (int i = 0; i < movies.Length; i++) {
    //            if (serverVotes == null) {
    //                //builder.AddField($"{(i + 1)}. {movies[i].Title}", $"**m!vote** {(i + 1)}\n---------- 0/0");
    //                builder.AddField ( $"{MojiCommand.voteEmojiCodes[i]} {movies[i].Title}",$"---------- 0/0" );
    //            } else {
    //                //Votes have been cast, build the votes embed
    //                string voteTally = "";
    //                int perc = (int)Math.Round ( ((float)votes[i] / serverVotes.Count) * 10 );//Gives me a value between 0 and 10
    //                for (int t = 1;t <= 10;t++) {
    //                    //0 means no x, greater than 0 means x will be shown.
    //                    voteTally += (perc >= t) ? "x" : "-";
    //                }
    //                voteTally += $" {votes[i]} / {serverVotes.Count}";
    //                builder.AddField ( $"{MojiCommand.voteEmojiCodes[i]} {movies[i].Title}",$"{voteTally}" );
    //            }
    //        }
    //        DateTime now = DateTime.UtcNow;
    //        string fuu = $"{now.Month}/{now.Day}/{now.Year} 21:00:00";
    //        now = DateTime.Parse(fuu);
    //        DateTimeOffset offset = new DateTimeOffset(now, new TimeSpan(0, 0, 0));
    //        builder.WithTimestamp(offset);
    //        //builder.AddField("React with " + MojiCommand.commandEmojiCodes[0] + " to end the current vote.", "-");
    //        return builder.Build ();
    //    }

    //    private static int[] GetVoteTotals ( Dictionary<string,int> serverVotes,Movie[] movies ) {
    //        int[] votes = new int[movies.Length];
    //        //This ends the vote, then tallies up the numbers.
    //        foreach (KeyValuePair<string,int> entry in serverVotes) {
    //            votes[entry.Value] += 1;//Tally up the votes
    //        }
    //        return votes;
    //    }
    //}
}
