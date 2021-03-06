﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieNightBot.Core.Commands {
    class MojiCommand {

        
        public volatile static string[] voteEmojiCodes = { "🇦", "🇧", "🇨", "🇩", "🇪", "🇫", "🇬", "🇭", "🇮", "🇯", "🇰", "🇱", "🇲", "🇳", "🇴", "🇵", "🇶", "🇷", "🇸", "🇹" };
        public volatile static string[] commandEmojiCodes = { "🛑", "🔄", "✔️", "⏮", "⏪", "⏩", "⏭" };

        private volatile static Emoji[] commandMojiStore;
        public static Emoji[] CommandMoji {
            get {
                if (commandMojiStore == null) {
                    commandMojiStore = new Emoji[commandEmojiCodes.Length];
                    for (int i = 0; i < commandEmojiCodes.Length; i++) {
                        commandMojiStore[i] = new Emoji(commandEmojiCodes[i]);
                    }
                }
                return commandMojiStore;
            }
        }

        private volatile static Emoji[] voteMojiStore;
        public static Emoji[] VoteMoji {
            get {
                if (voteMojiStore == null) {
                    voteMojiStore = new Emoji[voteEmojiCodes.Length];
                    for (int i = 0; i < voteEmojiCodes.Length; i++) {
                        voteMojiStore[i] = new Emoji(voteEmojiCodes[i]);
                    }
                }
                return voteMojiStore;
            }
        }

        //public static async Task Vote ( IUserMessage Message, ISocketMessageChannel channel, SocketReaction reaction ) {
        //    await Voting.Vote(Message, channel, reaction);
        //}

        public static async Task CommandEmoji ( IUserMessage Message, ISocketMessageChannel channel, SocketReaction reaction ) {
            //The only reason (for now) to use this is if a vote is in need of stopping.
            //Check if the server is correct?
            if (reaction.Emote.Equals(CommandMoji[0])) {
                //Stop command.
            }
        }

        public static bool IsReset(IEmote moji) {
            return moji.Equals((CommandMoji[1] as IEmote));
        }

        public static bool IsStop ( IEmote moji ) {
            return moji.Equals(( CommandMoji[0] as IEmote ));
        }

        public static bool IsLeftEnd( IEmote moji) {
            return moji.Equals(( CommandMoji[3] as IEmote ));
        }

        public static bool IsLeft ( IEmote moji ) {
            return moji.Equals(( CommandMoji[4] as IEmote ));
        }

        public static bool IsRight ( IEmote moji ) {
            return moji.Equals(( CommandMoji[5] as IEmote ));
        }

        public static bool IsRightEnd ( IEmote moji ) {
            return moji.Equals(( CommandMoji[6] as IEmote ));
        }

        public static int EmojiToVoteNumber ( IEmote moji ) {
            int i = 0;
            foreach (IEmote emoji in VoteMoji) {
                if (moji.Equals(emoji)) {
                    return i;
                }
                i++;
            }
            return -1;
        }

    }
}
