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
    public class RankedServerVoting: ModuleBase<SocketCommandContext> {

        public static volatile Dictionary<ulong, RankedServerVote> ServersAndVotes;

        [Command("start_vote"), Summary("Starts a new ranked vote.")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task NewVote () {
            try {
                //Create instance of servers and votes if it does not already exist.
                if (ServersAndVotes == null) { ServersAndVotes = new Dictionary<ulong, RankedServerVote>(); }

                //Vote has already started, do not continue
                if (ServersAndVotes.ContainsKey(Context.Guild.Id)) {
                    await Context.Channel.SendMessageAsync("A vote is already running, please end the current one before starting a new one!");
                    return;
                }
                //Get the movies to vote on
                Movie[] movs = MoviesData.Model.GetRandomVote(Context.Guild);
                //Generate the ranked vote object
                RankedServerVote serverVote = new RankedServerVote(Context.Guild, movs, Context.Channel);

                //build and send out the voting embed
                Embed embed = serverVote.MakeVoteEmbed();
                //Send the message out
                RestUserMessage mess = await Context.Channel.SendMessageAsync("Movie Vote!!!", embed: embed).ConfigureAwait(false);
                Emoji[] votes = new Emoji[movs.Length+1];
                //React with the voting reactions
                for (int i = 0; i < movs.Length; i++) {
                    votes[i] = MojiCommand.VoteMoji[i];
                }
                votes[votes.Length - 1] = MojiCommand.CommandMoji[1];
                await mess.AddReactionsAsync(votes);

                //Finally add this vote object to the dictionary for later reference
                serverVote.voteMessage = mess;
                ServersAndVotes.Add(Context.Guild.Id, serverVote);
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("end_vote"), Summary("Ends a previously started vote.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task EndVote() {
            try {
                //Now, tally the votes and show the results
                if (ServersAndVotes == null) {
                    ServersAndVotes = new Dictionary<ulong, RankedServerVote>();
                    await Context.Channel.SendMessageAsync("Unable to end vote. No vote is currently running!");
                    return;
                }

                //Vote has already started, do not continue
                if (!ServersAndVotes.ContainsKey(Context.Guild.Id)) {
                    await Context.Channel.SendMessageAsync("Unable to end vote. No vote is currently running!");
                    return;
                }

                //Now tally and respond (response is automagic
                await ServersAndVotes[Context.Guild.Id].TallyResults();
                //That should be it then! The rest is taken care of by the object!!! Horay for OOP!
                ServersAndVotes.Remove(Context.Guild.Id);

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
