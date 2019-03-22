using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MovieNightBot.Core.Data;

namespace MovieNightBot.Core.Moderation {
    public class ServerSettings: ModuleBase<SocketCommandContext> {
        public volatile static string[] CommandNames = new string[] { "help", "suggest", "watched" };

        [Command("set_admin_role"), Summary("Set the name of the role that is allowed to do stuff with the bot.")]
        public async Task SetAdminRoleName ( [Remainder]string Input = "" ) {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == MoviesData.Model.GetAdminRoleName(Context.Guild));

                if (user.Roles.Contains(role)) {
                    if (Input == "") {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, you can't set the admin role name to nothing! You dunce!");
                    }
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the admin role name has been updated to {Input}! Please ensure you have this role, or you can't execute administrative commands with me!");
                    MoviesData.Model.SetAdminRoleName(Context.Guild, Input);
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("get_admin_role"), Summary("Gets the name of the role that is allowed to do stuff with the bot.")]
        public async Task GetAdminRoleName () {
            try {
                await Context.User.SendMessageAsync($"The name of the administrative role is currently {MoviesData.Model.GetAdminRoleName(Context.Guild)}.");
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("user_vote_count"), Summary("Set the name of the role that is allowed to do stuff with the bot.")]
        public async Task VoteCount ( [Summary("The number of movies a user may vote on.")] int number = 5 ) {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == MoviesData.Model.GetAdminRoleName(Context.Guild));

                if (user.Roles.Contains(role)) {
                    if (number < 1) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, the number of votes must be greater than 0.");
                        return;
                    }
                    if (number > MoviesData.Model.GetMovieOptionCount(Context.Guild)) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, the number of votes must be greater than the number of movies that show in a vote. To modify the number of movies in a vote use **m!movie_vote_count**");
                        return;
                    }
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, users will be allowed to place {number} votes.");
                    MoviesData.Model.SetVoteCount(Context.Guild, number);
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("tie_option"), Summary("Set how the bot handles ties.")]
        public async Task SetTieOption ( [Remainder]string Input = "" ) {//Because we could all use some from time to time.
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == MoviesData.Model.GetAdminRoleName(Context.Guild));
                if (user.Roles.Contains(role)) {
                    int option = 0;
                    switch (Input) {
                        case "breaker":
                        option = 0;
                        break;
                        case "random":
                        option = 1;
                        break;
                        default:
                        await Context.User.SendMessageAsync("Unknown argument. For **m!tieoption** the options are *breaker* and *random*. Use **m!help** for more detials.");
                        return;
                    }

                    if (user.Roles.Contains(role)) {
                        //This user is allowed to configure settings.
                        MoviesData.Model.SetTiebreakerOption(Context.Guild, option);
                        await Context.User.SendMessageAsync($"MovieNightBot will now use the {Input} method for ties.");
                    }
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("movie_option_count")]
        [Summary("Sets the number of movies that will be selected for a vote.")]
        public async Task SetMovieCount ( [Summary("The number of movies to be selected.")] int number = 5 ) {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == MoviesData.Model.GetAdminRoleName(Context.Guild));

                if (user.Roles.Contains(role)) {
                    try {
                        //int count = -1;
                        //Input sanitization and filtering
                        if (number.Equals("")) { Console.WriteLine("Empty argument."); return; }//Empty argument

                        if (number < 2) {//Valid range check
                            Console.WriteLine("Can't be less than 2.");
                            await Context.Channel.SendMessageAsync($"{Context.User.Username}, vote count can't be less than 2.");
                            return;
                        }

                        if (number < MoviesData.Model.GetVoteCount(Context.Guild)) {
                            await Context.Channel.SendMessageAsync($"{Context.User.Username}, the number of movie options on a vote cannot be smaller than the number of movies a user is allowed to vote for!.");
                            return;
                        }

                        //Set the limit in the server file.
                        MoviesData.Model.SetMovieOptionCount(Context.Guild, number);
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, from now on {number} movies will show up for votes.");
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                } else {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, you need to have the role {MoviesData.Model.GetAdminRoleName(Context.Guild)} to use this command.");
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }
    }
}
