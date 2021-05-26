using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace BotSRT.Discord.Commands
{
    public class Setups : BaseCommandModule
    {
        public enum Mode
        {
            Car,
            Circuit,
            Season,
            Setup
        }

        private const int ITEMS_TO_SHOW = 4;

        private ulong currentEmbedId;
        private string currentPath;
        private List<string> nextDirectories;

        private int mode = 0;
        private int startIndex = 0;

        [Command("setups")]
        public async Task SetupsCommand(CommandContext context)
        {
            if (context.Channel.Id.ToString() != Environment.GetEnvironmentVariable("SetupsChannelId"))
                return;

            mode = 0;
            startIndex = 0;
            currentPath = @".\Setups";
            nextDirectories = GetNextDirectories().ToList();
            if (!nextDirectories.Any())
                return;

            var embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Yellow,
                Title = "🛠️ Setups",
                Description = GetDirectoriesToShow(),
                Thumbnail = new EmbedThumbnail
                {
                    Url = "https://i.imgur.com/OdnZfqp.png"
                },
                Footer = new EmbedFooter
                {
                    Text = $"v{Environment.GetEnvironmentVariable("Version")}"
                },
                Timestamp = DateTime.Now
            };

            embedBuilder.AddField("Ruta", currentPath);
            var totalPages = Math.Round(Convert.ToDecimal(nextDirectories.Count) / Convert.ToDecimal(ITEMS_TO_SHOW));
            embedBuilder.AddField("Página", $"{(startIndex / ITEMS_TO_SHOW) + 1}/{(totalPages == 0 ? 1 : totalPages)}");

            var embedMessage = embedBuilder.Build();
            var message = await context.Channel.SendMessageAsync(embedMessage);
            currentEmbedId = message.Id;
            await ReactWithPagination(message);
            await AwaitReaction(context, message);
        }

        private async Task ReactWithPagination(DiscordMessage message)
        {
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("⬅️"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("1️⃣"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("2️⃣"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("3️⃣"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("4️⃣"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("➡️"));
        }

        private async Task AwaitReaction(CommandContext context, DiscordMessage message)
        {
            var result = await message.WaitForReactionAsync(context.Member);

            if (currentEmbedId != message.Id)
                return;

            await message.DeleteReactionAsync(result.Result.Emoji, context.Member);

            switch (result.Result.Emoji)
            {
                case "⬅️":
                    var pIdx = startIndex - ITEMS_TO_SHOW;
                    startIndex = (pIdx < 0) ? 0 : pIdx;
                    message = await UpdateMessage(message);
                    await AwaitReaction(context, message);
                    break;
                case "1️⃣":
                    await GoToNextDirectory(context, message, startIndex);
                    break;
                case "2️⃣":
                    await GoToNextDirectory(context, message, startIndex + 1);
                    break;
                case "3️⃣":
                    await GoToNextDirectory(context, message, startIndex + 2);
                    break;
                case "4️⃣":
                    await GoToNextDirectory(context, message, startIndex + 3);
                    break;
                case "➡️":
                    var nIdx = startIndex + ITEMS_TO_SHOW;
                    startIndex = (nIdx >= nextDirectories.Count) ? startIndex : nIdx;
                    message = await UpdateMessage(message);
                    await AwaitReaction(context, message);
                    break;
            }
        }

        private async Task<DiscordMessage> UpdateMessage(DiscordMessage message)
        {
            var embedBuilder = new DiscordEmbedBuilder(message.Embeds.First());
            embedBuilder.Description = GetDirectoriesToShow();
            embedBuilder.ClearFields();
            embedBuilder.AddField("Ruta", currentPath);
            var totalPages = Math.Round(Convert.ToDecimal(nextDirectories.Count) / Convert.ToDecimal(ITEMS_TO_SHOW));
            embedBuilder.AddField("Página", $"{(startIndex / ITEMS_TO_SHOW) + 1}/{(totalPages == 0 ? 1 : totalPages)}");
            var result = await message.ModifyAsync(embedBuilder.Build());
            return result;
        }

        private IEnumerable<string> GetNextDirectories()
        {
            if (Directory.Exists(currentPath))
            {
                var directories = Directory.GetDirectories(currentPath);
                foreach(var directory in directories)
                {
                    yield return Path.GetFileName(directory);
                }
            }
        }

        private string GetDirectoriesToShow()
        {
            int index = 0;
            var result = "";
            var parseItemsToShow = startIndex + ITEMS_TO_SHOW >= nextDirectories.Count 
                ? nextDirectories.Count - startIndex 
                : ITEMS_TO_SHOW;
            foreach (var directory in nextDirectories.GetRange(startIndex, parseItemsToShow))
            {
                result = string.Concat(result, $"{++index}.- {directory.ToUpperInvariant()}\n");
            }
            return result;
        }

        private async Task GoToNextDirectory(CommandContext context, DiscordMessage message, int index)
        {
            mode++;
            currentPath = Path.Combine(currentPath, nextDirectories[index]);
            startIndex = 0;

            if ((Mode)mode == Mode.Setup)
            {
                await PrepareToDownloadSetups(context, message);
            }
            else
            {
                nextDirectories = GetNextDirectories().ToList();
                if (!nextDirectories.Any())
                    throw new DirectoryNotFoundException();

                message = await UpdateMessage(message);
                await AwaitReaction(context, message);
            }
        }

        private async Task PrepareToDownloadSetups(CommandContext context, DiscordMessage message)
        {
            var embedBuilder = new DiscordEmbedBuilder(message.Embeds.First());
            embedBuilder.Description = "Setup seleccionada correctamente...";
            embedBuilder.ClearFields();
            embedBuilder.AddField("Ruta", currentPath);
            message = await message.ModifyAsync(embedBuilder.Build());
            await message.DeleteAllReactionsAsync();
            await DownloadSetups(context);
        }

        private async Task DownloadSetups(CommandContext context)
        {
            if (Directory.Exists(currentPath))
            {
                var files = Directory.GetFiles(currentPath, "*.sto");
                foreach (var file in files)
                {
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        await new DiscordMessageBuilder()
                            .WithFiles(new Dictionary<string, Stream>() { { Path.GetFileName(file), fs } })
                            .SendAsync(context.Channel);
                    }
                }
            }
        }
    }
}
