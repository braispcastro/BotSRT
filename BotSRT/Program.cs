using BotSRT.Discord;
using BotSRT.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BotSRT
{
    class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var json = File.ReadAllText("config.json");
            var configuration = JsonConvert.DeserializeObject<Configuration>(json);
            SetConfiguration(configuration);

            var bot = new Bot();
            await bot.Start(configuration.DiscordToken);
        }

        private void SetConfiguration(Configuration configuration)
        {
            Environment.SetEnvironmentVariable("Version", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);

            Environment.SetEnvironmentVariable("DiscordApplicationId", configuration.DiscordApplicationId);
            Environment.SetEnvironmentVariable("DiscordClientId", configuration.DiscordClientId);
            Environment.SetEnvironmentVariable("DiscordClientSecret", configuration.DiscordClientSecret);
            Environment.SetEnvironmentVariable("DiscordPublicKey", configuration.DiscordPublicKey);
            Environment.SetEnvironmentVariable("DiscordToken", configuration.DiscordToken);
            Environment.SetEnvironmentVariable("SetupsChannelId", configuration.SetupsChannelId);
        }
    }
}
