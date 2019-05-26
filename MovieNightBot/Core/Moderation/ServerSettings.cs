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
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);

                if (user.Roles.Contains(role)) {
                    if (Input == "") {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, you can't set the admin role name to nothing! You dunce!");
                    }
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the admin role name has been updated to {Input}! Please ensure you have this role, or you can't execute administrative commands with me!");
                    sd.AdminRoleName = Input;
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
                ServerData sd = ServerData.Get(Context.Guild);
                await Context.User.SendMessageAsync($"The name of the administrative role is currently {sd.AdminRoleName}.");
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
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);
                if (user.Roles.Contains(role)) {
                    if (number < 1) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, the number of votes must be greater than 0.");
                        return;
                    }
                    if (number > sd.MovieVoteOptionCount) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, the number of votes must be greater than the number of movies that show in a vote. To modify the number of movies in a vote use **m!movie_vote_count**");
                        return;
                    }
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, users will be allowed to place {number} votes.");
                    sd.UserVoteLimit = number;
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
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);
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
                        sd.TiebreakerMethod = option;
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
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);

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

                        if (number < sd.UserVoteLimit) {
                            await Context.Channel.SendMessageAsync($"{Context.User.Username}, the number of movie options on a vote cannot be smaller than the number of movies a user is allowed to vote for!.");
                            return;
                        }

                        //Set the limit in the server file.
                        sd.MovieVoteOptionCount = number;
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, from now on {number} movies will show up for votes.");
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                } else {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, you need to have the role {sd.AdminRoleName} to use this command.");
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("set_movie_time"), Summary("Set the time that shows in the movie vote embed. Set the hour in UTC time.")]
        public async Task SetMovieTime ( [Summary("The number of movies to be selected.")] int number = 1 ) {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);

                if (user.Roles.Contains(role)) {
                    if (number < 0 || number > 23) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username} the time must be within the range of 0 to 23!");
                    }
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie time will now show as {number}! This will show up in the embed timestamp converted to user's local time zone!");
                    sd.MovieTimeHour = number;
                }
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Server Settings", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("set_drunko_mode"), Summary("Keeps the drunks from making shit suggestions.")]
        public async Task SetDrunkoMode ( [Summary("The number of movies to be selected.")] int enabled = 0 ) {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);

                if (user.Roles.Contains(role)) {
                    if (enabled < 0 || enabled > 1) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username} the setting should be either 0 or 1!");
                    }
                    if (enabled == 0) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, movie suggestions are re-enabled.");
                        sd.DrunkoModeEnabled = false;
                    } else {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, movie suggestions are disabled temporarily.");
                        sd.DrunkoModeEnabled = true;
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
    }
}
