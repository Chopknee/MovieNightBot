using System.Threading.Tasks;
using MovieNightBot.Core.Data;
using Discord.Commands;
using Discord;
using System.Collections.Generic;
using Discord.Rest;
using System;

namespace MovieNightBot.Core.Commands {
    public class MoviesInfo : ModuleBase<SocketCommandContext> {

        public static volatile Dictionary<ulong, ShowMovieSuggestions> serversAndSuggestionsEmbeds;
        public static volatile Dictionary<ulong, ShowMovieSuggestions> serversAndWatchedEmbeds;

        [Command("watched"), Summary("Show an embed which allows users to see what movies have been watched.")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task ListWatchedMovies ( [Summary("The page number to start on.")] int number = 1 ) {
            try {
                if (serversAndWatchedEmbeds == null) {
                    serversAndWatchedEmbeds = new Dictionary<ulong, ShowMovieSuggestions>();
                }
                if (!serversAndWatchedEmbeds.ContainsKey(Context.Guild.Id)) {
                    ShowMovieSuggestions sugg = new ShowMovieSuggestions(Context.Guild, Context.Channel, true);
                    sugg.pageNumber = number - 1;
                    serversAndWatchedEmbeds.Add(Context.Guild.Id, sugg);

                    RestUserMessage mess = await Context.Channel.SendMessageAsync("See Watched Movies", embed: sugg.MakeEmbed()).ConfigureAwait(false);
                    Emoji[] votes = new Emoji[4];
                    votes[0] = MojiCommand.CommandMoji[3];
                    votes[1] = MojiCommand.CommandMoji[4];
                    votes[2] = MojiCommand.CommandMoji[5];
                    votes[3] = MojiCommand.CommandMoji[6];
                    await mess.AddReactionsAsync(votes);
                    sugg.suggestionsMessage = mess;
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("suggested"), Summary("Show all suggested movies.")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task ListSuggestedMovies ( [Summary("The page number to start on.")] int number = 1 ) {
            try {
                if (serversAndSuggestionsEmbeds == null) {
                    serversAndSuggestionsEmbeds = new Dictionary<ulong, ShowMovieSuggestions>();
                }
                if (!serversAndSuggestionsEmbeds.ContainsKey(Context.Guild.Id)) {
                    ShowMovieSuggestions sugg = new ShowMovieSuggestions(Context.Guild, Context.Channel, false);
                    sugg.pageNumber = number - 1;
                    serversAndSuggestionsEmbeds.Add(Context.Guild.Id, sugg);

                    RestUserMessage mess = await Context.Channel.SendMessageAsync("See Suggested Movies", embed: sugg.MakeEmbed()).ConfigureAwait(false);
                    Emoji[] votes = new Emoji[4];
                    votes[0] = MojiCommand.CommandMoji[3];
                    votes[1] = MojiCommand.CommandMoji[4];
                    votes[2] = MojiCommand.CommandMoji[5];
                    votes[3] = MojiCommand.CommandMoji[6];
                    await mess.AddReactionsAsync(votes);
                    sugg.suggestionsMessage = mess;
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }
    }
}
