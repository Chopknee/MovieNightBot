using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MovieNightBot.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace MovieNightBot.Core.Data {
    public class ShowMovieSuggestions {

        ISocketMessageChannel channel;
        SocketGuild guild;

        public RestUserMessage suggestionsMessage;
        private ServerData serverData;

        bool showWatched;

        int pageSize = 15;

        int totalPages;
        public int pageNumber = 0;

        Timer expirationTimer;

        int timeoutTime = 5;//In minuites

        public ShowMovieSuggestions( SocketGuild guild, ISocketMessageChannel channel, bool showWatched ) {
            this.channel = channel;
            this.guild = guild;
            serverData = ServerData.Get(guild);
            this.showWatched = showWatched;
            Program.SubscribeToReactionRemoved(Action);
            Program.SubscribeToReactionAdded(Action);
            totalPages = showWatched ? serverData.GetWatchedMovies().Count() : serverData.GetSuggestedMovies().Count();
            totalPages = (int)Math.Ceiling(totalPages  / (float)pageSize);
            pageNumber = Math.Min(totalPages - 1, pageNumber);
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            expirationTimer = new Timer(OnExpire, autoEvent, 1000 * 60 * timeoutTime, Timeout.Infinite);
            Program.Instance.OnMoviesListModified += OnMoviesModified;
            
        }

        public async Task Action ( Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction ) {
            if (this.channel.Id == channel.Id && reaction.MessageId == suggestionsMessage.Id && reaction.UserId != Program.Instance.client.CurrentUser.Id) {
                int old = pageNumber;
                //
                if (MojiCommand.IsLeftEnd(reaction.Emote)) {
                    //Show the first page
                    old = 0;
                }

                if (MojiCommand.IsLeft(reaction.Emote)) {
                    //Show the previous page
                    old -= 1;
                }

                if (MojiCommand.IsRight(reaction.Emote)) {
                    //Show the next page
                    old += 1;
                }

                if (MojiCommand.IsRightEnd(reaction.Emote)) {
                    //Show the last page
                    old = totalPages - 1;
                }

                if (old != pageNumber && old >= 0 && old <= totalPages) {
                    pageNumber = old;
                    await suggestionsMessage.ModifyAsync(VoteMessage => {
                        VoteMessage.Content = "Moves Info!!";
                        VoteMessage.Embed = MakeEmbed();
                        return;
                    });
                }
            }
        }

        public Embed MakeEmbed() {
            int voteCount = serverData.UserVoteLimit;//MoviesData.Model.GetVoteCount(associatedGuild);
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle(( ( showWatched ) ? "Watched" : "Suggested" ) + "Movie Information")
                .WithDescription($"Look at all the movies that have been " + ((showWatched)? "watched" : "suggested") + "!")
                .WithColor(new Color(0xE314C7))
                .WithTimestamp(DateTime.Now)
                .WithAuthor(author => {
                    author
                    .WithName("Movie Night Bot");
                });
            IEnumerable<Movie> movs;
            if (showWatched) {
                movs = from m in serverData.GetWatchedMovies() select m;
            } else {
                //movs = from m in serverData.GetSuggestedMovies() select m;
                //IEnumerable<float> scores = from suggestion in serverData.GetSuggestedMovies()
                //                            where suggestion.Watched == false
                //                            select suggestion.ClassificationScore;
                //float max = scores.Max();

                ////Order the list based on the scoring system.
                //movs = from suggestion in serverData.GetSuggestedMovies()
                //                           where suggestion.Watched == false
                //                           orderby ( ( suggestion.ClassificationScore == -1 ) ? max : suggestion.ClassificationScore )//score
                //                           select suggestion;
                movs = from suggestion in serverData.GetSuggestedMovies()
                       where suggestion.Watched == false
                       orderby suggestion.Title
                       select suggestion;
            }

            movs = movs.Skip(pageNumber * pageSize);
            movs = movs.Take(pageSize);

            if (showWatched) {
                foreach (Movie mov in movs) {
                    builder.AddField($"{mov.Title}", $"Watched: {mov.WatchedDate}");
                }
            } else {
                foreach (Movie mov in movs) {
                    builder.AddField($"{mov.Title}", $"**Votes:** {mov.TotalVotes} |**Score:** {mov.TotalScore} |**Times Up for Vote:** {mov.TimesUpForVote}");
                }
            }
            builder.AddField("Use reactions to navigate the pages!", $"Page {(pageNumber + 1)} of {totalPages}");
            return builder.Build();
        }

        public async void OnExpire(Object stateInfo) {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"This embed has expired!")
                .WithDescription($"You need to run the m!suggested or m!watched command to get this back.")
                .WithColor(new Color(0xE314C7));

            try {
                await suggestionsMessage.RemoveAllReactionsAsync();
            } catch (Exception ex) {
                //We don't really want to do anything. This is to catch the servers which don't give permissions to the bot.
                LogMessage lm = new LogMessage(LogSeverity.Error, "Movie Information Embed", "Unable to remove reactions from the movie information embed! The server likely has not given permission to do this.");
                await Program.Instance.Log(lm);
            }
            if (showWatched) {
                MoviesInfo.serversAndWatchedEmbeds.Remove(guild.Id);
            } else {
                MoviesInfo.serversAndSuggestionsEmbeds.Remove(guild.Id);
            }

            expirationTimer.Dispose();

            await suggestionsMessage.ModifyAsync(VoteMessage => {
                VoteMessage.Content = "Movie Vote!!!";
                VoteMessage.Embed = builder.Build();
                return;
            });
        }

        public async Task OnMoviesModified ( Movie m, SocketGuild guild, ISocketMessageChannel channel, SocketUser user ) {
            //Update the data and embed
            totalPages = showWatched ? serverData.GetWatchedMovies().Count() : serverData.GetSuggestedMovies().Count();
            totalPages = (int) Math.Ceiling(totalPages / (float) pageSize);
            pageNumber = Math.Min(totalPages - 1, pageNumber);
            await suggestionsMessage.ModifyAsync(VoteMessage => {
                VoteMessage.Content = "Moves Info!!";
                VoteMessage.Embed = MakeEmbed();
                return;
            });
        }

        ~ShowMovieSuggestions () {
            Program.UnSubscribeReactionAdded(Action);
            Program.UnSubscribeToReactionRemoved(Action);
            Program.Instance.OnMoviesListModified += OnMoviesModified;
        }
    }
}
