using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MovieNightBot.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieNightBot.Core.Data {
    public class RankedServerVote {

        //All the available options for this vote.
        Movie[] movieOptions;
        //
        ServerData serverData;

        //The guild which created the vote
        SocketGuild associatedGuild;
        ISocketMessageChannel channel;

        //All the users with votes
        Dictionary<ulong, List<int>> voters;

        //The message that shows the current vote.
        public RestUserMessage voteMessage;

        //The message that shows the status of each user ballot
        public RestUserMessage feedbackMessage;



        private int numVotes = 0;
        private struct BallotItem {
            public Movie movie;
            public int votes;
            public float score;
        }

        BallotItem[] ballotItems;

        public RankedServerVote(SocketGuild guild, Movie[] movieOptions, ISocketMessageChannel channel) {
            this.associatedGuild = guild;
            this.channel = channel;
            voters = new Dictionary<ulong, List<int>>();
            this.movieOptions = movieOptions;
            Program.SubscribeToReactionAdded(ReactCallback);
            Program.SubscribeToReactionRemoved(UnReactCallback);
            //ServerData data = ServerData.Get(guild);
            //maxUserVotes = MoviesData.Model.GetVoteCount(guild);
            ballotItems = new BallotItem[movieOptions.Length];
            serverData = ServerData.Get(guild);

            for (int i = 0; i < ballotItems.Length; i++) {
                ballotItems[i] = new BallotItem();
                ballotItems[i].movie = movieOptions[i];
                ballotItems[i].votes = 0;
                ballotItems[i].score = 0;
            }

        }

        //This will be called whenever
        public async Task ReactCallback( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            try {
                //Need to check if the emoji even belongs to this server, and if the message being reacted to is the correct one.
                if (this.channel.Id == channel.Id && reaction.MessageId == voteMessage.Id && reaction.UserId != Program.Instance.client.CurrentUser.Id) {
                    //Check how to deal with this reaction.
                    if (VerifyAsVote(reaction)) {
                        await PlaceVote(userMessage, channel, reaction);
                    }

                    //This is the reset command
                    if (MojiCommand.IsReset(reaction.Emote)) {
                        await ResetVote(userMessage, channel, reaction);
                    }

                    if (MojiCommand.IsStop(reaction.Emote)) {
                        await EndVote(userMessage, channel, reaction);
                    }
                }
            } catch(Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "React Callback Voting", "An unknown error occurred.", ex));
            }
        }

        public async Task UnReactCallback( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            try {
                //Need to check if the emoji even belongs to this server, and if the message being reacted to is the correct one.
                if (this.channel.Id == channel.Id && reaction.MessageId == voteMessage.Id && reaction.UserId != Program.Instance.client.CurrentUser.Id) {
                    //Check how to deal with this reaction.
                    if (VerifyAsVote(reaction)) {
                        await PlaceVote(userMessage, channel, reaction);
                    }

                    //This is the reset command
                    if (MojiCommand.IsReset(reaction.Emote)) {
                        await ResetVote(userMessage, channel, reaction);
                    }
                }
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "React Remove Callback Voting", "An unknown error occurred.", ex));
            }
        }

        private async Task PlaceVote( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            if (!voters.ContainsKey(reaction.UserId)) {
                voters.Add(reaction.UserId, new List<int>());
            }
            List<int> ballot = voters[reaction.UserId];
            if (ballot.Count >= serverData.UserVoteLimit) {
                //We need to send the user a fraggin message??? Nope. Maxed out votes, ignore this particular reaction?? Assuming it is even a vote... (which it may very well not be)
                return;
            }

            int vote = MojiCommand.EmojiToVoteNumber(reaction.Emote);
            if (vote >= movieOptions.Length || ballot.Contains(vote)) {
                //User tried to vote for something outside the valid range, or they already voted for this
                return;
            }

            //If the execution makes it here, we can safely add the vote to the ballot
            voters[reaction.UserId].Add(vote);

            numVotes = CalculateBallotScore();

            //Update any embeds here!
            await voteMessage.ModifyAsync(VoteMessage => {
                VoteMessage.Content = "Movie Vote!!!";
                VoteMessage.Embed = MakeVoteEmbed();
                return;
            } );
        }

        private async Task ResetVote( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            //This is where the user may reset their vote.
            if (!voters.ContainsKey(reaction.UserId)) {
                voters.Add(reaction.UserId, new List<int>());
                //Stop doing anything because the user has nothing to clear.
                return;
            }

            voters[reaction.UserId].Clear();
            numVotes = CalculateBallotScore();
            //Update any embeds here!
            await voteMessage.ModifyAsync(VoteMessage => {
                VoteMessage.Content = "Movie Vote!!!";
                VoteMessage.Embed = MakeVoteEmbed();
                return;
            });
        }

        private async Task EndVote( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction) {
            //Possibly an admin only command.
            await TallyResults();

        }

        public Embed MakeVoteEmbed () {
            int voteCount = serverData.UserVoteLimit;//MoviesData.Model.GetVoteCount(associatedGuild);
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("What movie will we watch tonight?")
                .WithDescription($"You may choose up to {voteCount} movie" + ((voteCount > 1)? "s" : "") + ". React in the order of movies you want to view the most first.")
                .WithColor(new Color(0xE314C7))
                .WithTimestamp(DateTime.Now)
                .WithAuthor(author => {
                    author
                    .WithName("Movie Night Bot");
                });
            for (int i = 0; i < movieOptions.Length; i++) {
                if (voters.Count == 0) {
                    builder.AddField($"{MojiCommand.voteEmojiCodes[i]} {movieOptions[i].Title}", $"---------- 0/0");
                } else {
                    //Votes have been cast, build the votes embed
                    string voteTally = "";
                    int perc = (int) Math.Round(( (float) ballotItems[i].votes / numVotes ) * 10);//Gives me a value between 0 and 10
                    for (int t = 1; t <= 10; t++) {
                        //0 means no x, greater than 0 means x will be shown.
                        voteTally += ( perc >= t ) ? "x" : "-";
                    }
                    voteTally += $" {ballotItems[i].votes} / {numVotes}";
                    builder.AddField($"{MojiCommand.voteEmojiCodes[i]} {movieOptions[i].Title}", $"{voteTally}");
                }
            }
            DateTime now = DateTime.UtcNow;
            string fuu = $"{now.Month}/{now.Day}/{now.Year} {serverData.MovieTimeHour}:00:00";
            now = DateTime.Parse(fuu);
            DateTimeOffset offset = new DateTimeOffset(now, new TimeSpan(0, 0, 0));
            builder.WithTimestamp(offset);
            builder.AddField($"React with {MojiCommand.commandEmojiCodes[1]} to reset your vote.", $"React with {MojiCommand.commandEmojiCodes[0]} to end the current vote.\nThe movie will start at - ");
            return builder.Build();
        }

        //Tallies the result and displays the outcome in the original embed
        public async Task TallyResults () {
            //Recalculate the current ballot
            int votes = CalculateBallotScore();

            for (int i = 0; i < ballotItems.Length; i++) {
                ballotItems[i].movie.TotalScore += ballotItems[i].score;
                ballotItems[i].movie.TotalVotes += ballotItems[i].votes;
                ballotItems[i].movie.TimesUpForVote += 1;
            }

            //Determine the winner
            int maxWin = -1;
            for (int i = 0; i < ballotItems.Length; i++) {
                if (maxWin == -1) {
                    maxWin = i;
                    continue;
                }
                if (ballotItems[i].score > ballotItems[maxWin].score) {
                    maxWin = i;
                }
            }
            BallotItem winner = ballotItems[maxWin];
            //Show the winner in the old embed
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"The winning vote was {winner.movie.Title}!")
                .WithDescription($"To set the movie as watched use the command m!set_watched {winner.movie.Title}")
                .WithColor(new Color(0xE314C7));
            DateTime now = DateTime.UtcNow;
            string fuu = $"{now.Month}/{now.Day}/{now.Year} {serverData.MovieTimeHour}:00:00";
            now = DateTime.Parse(fuu);
            DateTimeOffset offset = new DateTimeOffset(now, new TimeSpan(0, 0, 0));
            builder.WithTimestamp(offset);
            //We dooone - Remove this from the current setup.
            RankedServerVoting.ServersAndVotes.Remove(associatedGuild.Id);
            try {
                await voteMessage.RemoveAllReactionsAsync();
            } catch (Exception ex) {//We don't really want to do anything. This is to catch the servers which don't give permissions to the bot.
            }

            await voteMessage.ModifyAsync(VoteMessage => {
                VoteMessage.Content = "Movie Vote!!!";
                VoteMessage.Embed = builder.Build();
                return;
            });

        }

        private int CalculateBallotScore() {
            //Reset old count
            int numVotes = 0;
            for (int i = 0; i < ballotItems.Length; i++) {
                ballotItems[i].score = 0;
                ballotItems[i].votes = 0;
            }

            //Generate new count
            foreach (KeyValuePair<ulong, List<int>> entry in voters) {
                float divisor = 1f / (float)serverData.UserVoteLimit;
                float weight = 1;
                foreach (int vt in entry.Value) {
                    ballotItems[vt].votes += 1;
                    ballotItems[vt].score += weight;
                    weight = weight - divisor;
                    numVotes++;
                }
            }
            return numVotes;
            //Order the list?
        }

        private bool VerifyAsVote(SocketReaction reaction ) {   
            foreach (IEmote moji in MojiCommand.VoteMoji) {
                if (moji.Equals(reaction.Emote)) {
                    //Tell the voting system that a user has requested a vote!!!
                    return true;
                }
            }
            return false;
        }

        ~RankedServerVote () {
            Program.UnSubscribeReactionAdded(ReactCallback);
            Program.UnSubscribeToReactionRemoved(UnReactCallback);
        }
    }
}
