using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BotSRT.Discord.Services
{
    public class ReactionHandlingService
    {
        private readonly CommandService commands;
        private readonly DiscordSocketClient discord;
        private readonly IServiceProvider services;

        public ReactionHandlingService(IServiceProvider services)
        {
            commands = services.GetRequiredService<CommandService>();
            discord = services.GetRequiredService<DiscordSocketClient>();
            this.services = services;

            discord.ReactionAdded += ReactionAdded;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
                return Task.CompletedTask;

            // Logic here...

            return Task.CompletedTask;
        }
    }
}
