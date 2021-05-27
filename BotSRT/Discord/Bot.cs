using BotSRT.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BotSRT.Discord
{
    public class Bot
    {
        public async Task Start(string token)
        {
            var options = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = LogLevel.Information,
                LogTimestampFormat = "dd/MM/yyyy hh:mm:ss"
            };

            var client = new DiscordClient(options);

            var services = new ServiceCollection()
                .AddSingleton<SetupsService>()
                .BuildServiceProvider();

            var commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableMentionPrefix = false,
                StringPrefixes = new[] { "!" },
                Services = services
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.KeepEmojis
            });

            client.Ready += Client_Ready;

            await client.ConnectAsync();
            await Task.Delay(-1);
        }

        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}] Conectado...");
            return Task.CompletedTask;
        }
    }
}
