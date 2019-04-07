using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using MovieNightBot.Core.Data;



namespace MovieNightBot.Core.Commands {
    //Setting / unsetting watched
    public class WatchStatus: ModuleBase<SocketCommandContext> {
        [Command("set_watched"), Summary("Sets a movie as having been watched. That movie will no longer show up in votes.")]
        public async Task SetAsWatched ( [Remainder]string Input = "" ) {
            try {
                //Input sanitization
                if (Input.Equals("") || Input.Length > 150) return; //Filter out bad input
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                Input = Input.Trim();//Clear spaces
                Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
                //Check if the movie has been suggested
                ServerData sd = ServerData.Get(Context.Guild);
                Movie m = sd.GetMovie(Input);
                if (m == null) {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been suggested yet.");
                    return;
                }
                if (m.Watched) {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has already been set to watched.");
                    return;
                }
                m.Watched = true;
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} is now set to watched and will no longer appear on votes.\nTo undo this, you can use **m!unwatch {Input}**.");
                Program.Instance.OnMoviesListModified?.Invoke(m, Context.Guild, Context.Channel, Context.User);
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("unwatch"), Summary("Returns a previously watched movie to the voting lists.")]
        public async Task SetUnwatched ( [Remainder]string Input = "" ) {
            try {
                //Input sanitization
                if (Input.Equals("") || Input.Length > 150) return; //Filter out bad input
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                Input = Input.Trim();//Clear spaces
                Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
                                                //Check if the movie has been suggested
                ServerData sd = ServerData.Get(Context.Guild);
                Movie m = sd.GetMovie(Input);
                if (m == null) {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been watched yet.");
                    return;
                }
                if (!m.Watched) {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been set to watched yet.");
                    return;
                }
                m.Watched = false;
                await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has been added back to the wait list and will show in future votes.\nTo undo this, you can use **m!set_watched {Input}**.");
                Program.Instance.OnMoviesListModified?.Invoke(m, Context.Guild, Context.Channel, Context.User);
            } catch (DataException ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A data related exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            } catch (Exception ex) {
                await Program.Instance.Log(new LogMessage(LogSeverity.Error, "Voting", "A general exception was raised.", ex));
                await Context.Channel.SendMessageAsync("I'm not really sure what happened but something went wrong while executing that command, sorry. :flushed:");
            }
        }

        [Command("remove"), Summary("Removes a movie from the lists completely.")]
        public async Task RemoveMovie ( [Remainder]string Input = "" ) {
            try {
                SocketGuildUser user = Context.User as SocketGuildUser;
                ServerData sd = ServerData.Get(Context.Guild);
                var role = ( user as IGuildUser ).Guild.Roles.FirstOrDefault(x => x.Name == sd.AdminRoleName);

                if (user.Roles.Contains(role)) {
                    //Input sanitization
                    if (Input.Equals("") || Input.Length > 150) return; //Filter out bad input
                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                    Input = Input.Trim();//Clear spaces
                    Input = myTI.ToTitleCase(Input);//Make it so every word starts with an upper case
                                                    //Check if the movie has been suggested

                    Movie m = sd.GetMovie(Input);
                    if (m == null) {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has not been suggested or watched.");
                        return;
                    }
                    sd.RemoveMove(Input);
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, the movie {Input} has been removed.");
                    Program.Instance.OnMoviesListModified?.Invoke(m, Context.Guild, Context.Channel, Context.User);
                } else {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, you need to have the role {sd.AdminRoleName} to use this command.");
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