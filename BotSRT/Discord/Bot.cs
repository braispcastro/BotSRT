using System;
using System.Threading;
using System.Threading.Tasks;
using BotSRT.Discord.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BotSRT.Discord
{
    public class Bot
    {
        private DiscordSocketClient client;

        public async Task Start(string token)
        {
            using (var services = ConfigureServices())
            {
                client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<ReactionHandlingService>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage msg)
        {
            var date = DateTime.UtcNow;
            Console.WriteLine($"{date.ToString("dd/MM/yyyy")} {msg.ToString()}");
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<ReactionHandlingService>()
                //.AddSingleton<HttpClient>()
                //.AddSingleton<PictureService>()
                .BuildServiceProvider();
        }
    }
}
