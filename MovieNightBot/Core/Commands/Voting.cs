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
        public static Dictionary<string, Movie[]> currentVotes;

        [Command("beginvote"), Summary("Start the voging process for a movie.")]
        public async Task BeginVote() {
            Movie[] movs = Movies.GetMovieVote(""+Context.Guild.Id, Context.Guild.Name);
            currentVotes["" + Context.Guild.Id] = movs;
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("What movie will we watch tonight?")
                .WithDescription("use m!vote [number/title] to add your vote! You may only vote once!")
                .WithColor(new Color(0xE314C7))
                .WithTimestamp(DateTime.Now)
                .WithAuthor(author => {
                    author
                    .WithName("Movie Night Bot");
                });
            for (int i = 0; i < movs.Length; i++) {
                builder.AddField(i+"", $"{movs[i].Title} suggested on {movs[i].suggestDate}");
            }
            Embed embed = builder.Build();
            await Context.Channel.SendMessageAsync("Movie Vote!!!", embed: embed).ConfigureAwait(false);

        }

        [Command("showvote"), Summary("Ends the voting process and shows the final result.")]
        public async Task ShowVote() {
        }

        [Command("vote"), Summary("Vote for one of the movies listed.")]
        public async Task Vote() {
        }
    }
}
