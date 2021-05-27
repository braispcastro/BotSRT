using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace BotSRT.Discord.Commands
{
    public class CurrentSeason : BaseCommandModule
    {
        [Command("currentseason")]
        public async Task CurrentSeasonCommand(CommandContext context)
        {
            await context.Channel.SendMessageAsync(Environment.GetEnvironmentVariable("CurrentSeason"));
        }
    }
}
