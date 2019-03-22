using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MovieNightBot.Core.Commands {
    public class Help : ModuleBase<SocketCommandContext> {
        public volatile static string[] CommandNames = new string[] { "help", "suggest", "watched" };

        [Command("help"), Summary("Get sum help")]
        public async Task GetHelp() {//Because we could all use some from time to time.
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("MovieNightBot Help")
                .WithDescription("I heard you asked for some help. We all need some from time to time, so here it is.\n" +
                "Commands marked with :no_entry: require the user to have the role \"Movie Master\", unless the server owner has changed it.\n" +
                "Valid command prefixes include m! M! or you may @ movie night bot.")
                .WithColor(new Color(0xFFFFFF))
                .WithTimestamp(DateTime.Now)
                .WithAuthor(author => {
                    author
                    .WithName("Movie Night Bot");
                });
            builder.AddField("m!help", "DMs the sender of the command this exact message. Now that's meta!");
            builder.AddField("m!suggest [Title]", "Adds the supplied movie to the suggestions list. There is a chance this movie will now show up on future votes.");
            builder.AddField("m!watched", "Lists all movies that have been watched.");
            builder.AddField("m!suggested", "Lists all movies that have been suggested.");
            builder.AddField("m!set_watched [Title]", "Sets the specified movie as having been watched. This movie will not show up on future votes.");
            builder.AddField("m!unwatch [Title]", "Removes the specified movie from the watched list.");
            builder.AddField("m!remove [Title]", ":no_entry:  Removes the specified movie from the suggestions list.");
            builder.AddField("m!start_vote", "Selects a number of random movie suggestions to be voted on.");
            //builder.AddField("m!make_vote", "Selects a number of random movie suggestions to be voted on.");
            builder.AddField("m!end_vote", $"Ends the currently running vote and displays the winning vote. Reacting to the vote embed with {MojiCommand.CommandMoji[0]} will also end the vote.");
            builder.AddField("m!user_vote_count [Number]", "Sets the number of movies users will be allowed to vote on. This cannot be greater than the number of movie options that shows on a vote.");
            builder.AddField("m!movie_option_count [Number]", ":no_entry:  Sets the number of movies that will show up on a vote.");
            builder.AddField("m!tie_option [option]", ":no_entry:  Sets how the bot handles tied votes.\n Option **breaker** will make a new vote using only the tied movies.\n" +
                "Option **random** will make a new vote with a random selection of movies.");
            builder.AddField("m!set_admin_role [Name]", ":no_entry: Sets the name of the role that is allowed to run admin only commands in movie night bot. WARNING, you will not be able to run commands if you have no users with this name!!!");
            builder.AddField("m!get_admin_role [Name]", "Gets the name of the role that is allowed to run admin only commands for Movie Night Bot. Use for emergencies.");
            Embed embed = builder.Build();
            await Context.User.SendMessageAsync("You Need Some Help", embed: embed).ConfigureAwait(false);
        }
    }
}
