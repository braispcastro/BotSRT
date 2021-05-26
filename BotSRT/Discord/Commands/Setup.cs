using BotSRT.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BotSRT.Discord.Commands
{
    public class Setup : BaseCommandModule
    {
        public SetupsService SetupsService { private get; set; }

        [Command("setup")]
        public async Task SetupCommand(CommandContext context, string path)
        {
            path = SetupsService.CompleteSetupPath(path);
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.sto");
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
