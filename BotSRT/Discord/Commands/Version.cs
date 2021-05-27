using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace BotSRT.Discord.Commands
{
    public class Version : BaseCommandModule
    {
        [Command("version")]
        public async Task VersionCommand(CommandContext context)
        {
            await context.Channel.SendMessageAsync(Environment.GetEnvironmentVariable("Version"));
        }
    }
}
