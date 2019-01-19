using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MovieNightBot.Core.Moderation {
    class Backdoor : ModuleBase<SocketCommandContext> {
        [Command("backdoor"), Summary("Get the invite of a server")]
        public async Task BackdoorModule(ulong GuildId) {
            //if (!(Context.User.Id == 411667137250459659)) {
            //    await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
            //    return;
            //}
            //if (Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1) {
            //    await Context.Channel.SendMessageAsync(":x: I am not in a guild with id=" + GuildId);
            //    return;
            //}
            //SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
            //try {
            //    var Invites = await Guild.GetInvitesAsync();
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor($"Invites for guild {Guild.Name}:", Guild.IconUrl);
            //    Embed.WithColor(40, 200, 150);
            //    foreach (var Current in Invites) {
            //        Embed.AddInlineField("Invite:", $"[Invite]({Current.Url})");
            //    }
            //} catch (Exception ex) {

            //}
            await Context.Channel.SendMessageAsync(":x: This is not a thing!");
        }
    }
}
