using System;
using System.Collections.Generic;
using System.Text;

namespace BotSRT.Model
{
    public class Configuration
    {
        public string DiscordApplicationId { get; set; }
        public string DiscordPublicKey { get; set; }
        public string DiscordClientId { get; set; }
        public string DiscordClientSecret { get; set; }
        public string DiscordToken { get; set; }
        public string SetupsChannelId { get; set; }
        public string CurrentSeason { get; set; }
    }
}
