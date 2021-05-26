using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotSRT.Discord.Commands
{
    public class Setups : ModuleBase<SocketCommandContext>
    {
        private int currentEmbedId;
        private string currentPath;

        [Command("setups")]
        public Task SetupsAsync()
        {
            if (Context.Channel.Id.ToString() != Environment.GetEnvironmentVariable("SetupsChannelId"))
                return Task.CompletedTask;

            currentPath = "./Setups";

            var embedBuilder = new EmbedBuilder
            {
                Color = Color.Gold,
                Title = "Setups",
                Description = "",
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"v{Environment.GetEnvironmentVariable("Version")}"
                },
                Timestamp = DateTime.Now
            };

            embedBuilder.AddField("Ruta", currentPath);

            var embedMessage = embedBuilder.Build();
            var message = ReplyAsync(embed: embedMessage);

            currentEmbedId = message.Id;
            var options = new RequestOptions { UseSystemClock = false };
            var emoteList = new List<IEmote>{ new Emoji("⬅️"), new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣"), new Emoji("4️⃣"), new Emoji("➡️") };
            message.Result.AddReactionsAsync(emoteList.ToArray(), options).ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}
