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

        //The guild which created the vote
        SocketGuild associatedGuild;
        ISocketMessageChannel channel;

        //All the users with votes
        Dictionary<ulong, List<int>> voters;

        //The message that shows the current vote.
        public RestUserMessage voteMessage;

        //The message that shows the status of each user ballot
        public RestUserMessage feedbackMessage;

        //Keeping this around to prevent calls to the data model
        private int maxUserVotes;

        public RankedServerVote(SocketGuild guild, Movie[] movieOptions, ISocketMessageChannel channel) {
            this.associatedGuild = guild;
            this.channel = channel;
            voters = new Dictionary<ulong, List<int>>();
            this.movieOptions = movieOptions;
            Program.SubscribeToReactionAdded(ReactCallback);
            Program.SubscribeToReactionRemoved(UnReactCallback);

            maxUserVotes = MoviesData.Model.GetVoteCount(guild);
        }

        //This will be called whenever
        public async Task ReactCallback( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            //Need to check if the emoji even belongs to this server, and if the message being reacted to is the correct one.
            if (this.channel.Id == channel.Id && reaction.MessageId == voteMessage.Id) {
                //Check how to deal with this reaction.
                if (VerifyAsVote(reaction)) {
                    await PlaceVote(userMessage, channel, reaction);
                }

                //This is the reset command
                if (MojiCommand.IsReset(reaction.Emote)) {
                    await ResetVote(userMessage, channel, reaction);
                }
            }
        }

        public async Task UnReactCallback( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            //Need to check if the emoji even belongs to this server, and if the message being reacted to is the correct one.
            if (this.channel.Id == channel.Id && reaction.MessageId == voteMessage.Id) {
                //Check how to deal with this reaction.
                if (VerifyAsVote(reaction)) {
                    await PlaceVote(userMessage, channel, reaction);
                }

                //This is the reset command
                if (MojiCommand.IsReset(reaction.Emote)) {
                    await ResetVote(userMessage, channel, reaction);
                }
            }
        }

        private async Task PlaceVote( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            if (!voters.ContainsKey(reaction.UserId)) {
                voters.Add(reaction.UserId, new List<int>());
            }
            List<int> ballot = voters[reaction.UserId];
            if (ballot.Count >= maxUserVotes) {
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

            //Update any embeds here!
            await voteMessage.ModifyAsync(VoteMessage => {
                VoteMessage.Content = "Movie Vote!!!";
                VoteMessage.Embed = MakeVoteEmbed();
                return;
            } );
        }

        private async Task ResetVote( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            //This is where the user may reset their vote.
        }

        public Embed MakeVoteEmbed () {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("What movie will we watch tonight?")
                .WithDescription($"React with the emoji next to the movie you wish to vote for.")
                .WithColor(new Color(0xE314C7))
                .WithTimestamp(DateTime.Now)
                .WithAuthor(author => {
                    author
                    .WithName("Movie Night Bot");
                });
            for (int i = 0; i < movieOptions.Length; i++) {
                if (voters.Count == 0) {
                    //builder.AddField($"{(i + 1)}. {movies[i].Title}", $"**m!vote** {(i + 1)}\n---------- 0/0");
                    builder.AddField($"{MojiCommand.voteEmojiCodes[i]} {movieOptions[i].Title}", $"---------- 0/0");
                } else {
                    //Votes have been cast, build the votes embed
                    //string voteTally = "";
                    //int perc = (int) Math.Round(( (float) votes[i] / serverVotes.Count ) * 10);//Gives me a value between 0 and 10
                    //for (int t = 1; t <= 10; t++) {
                    //    //0 means no x, greater than 0 means x will be shown.
                    //    voteTally += ( perc >= t ) ? "x" : "-";
                    //}
                    //voteTally += $" {votes[i]} / {serverVotes.Count}";
                    //builder.AddField($"{MojiCommand.voteEmojiCodes[i]} {movieOptions[i].Title}", $"{voteTally}");
                }
            }
            DateTime now = DateTime.UtcNow;
            string fuu = $"{now.Month}/{now.Day}/{now.Year} 21:00:00";
            now = DateTime.Parse(fuu);
            DateTimeOffset offset = new DateTimeOffset(now, new TimeSpan(0, 0, 0));
            builder.WithTimestamp(offset);
            //builder.AddField("React with " + MojiCommand.commandEmojiCodes[0] + " to end the current vote.", "-");
            return builder.Build();
        }

        //Tallies the result and displays the outcome in the original embed
        public Embed TallyResults () {
            //
            return null;
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
