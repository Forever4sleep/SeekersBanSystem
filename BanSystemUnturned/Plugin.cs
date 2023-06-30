using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace BanSystemUnturned {
    public class Plugin : RocketPlugin<Config> {

        public static Plugin Instance;
        private void TellBannedPlayersNumber() {
            Logger.Log($"There are {this.Configuration.Instance.BannedPlayers.Count} players on this server.");
        }

        public override TranslationList DefaultTranslations => new TranslationList() {
            {"one_argument", "You need to specify time and reason." },
            {"two_arguments", "You need to specify the ban reason." },
            {"no_player", "There is no such player as {0} present on the server right now." },
            {"no_ban", "You cannot ban {0} since they belong to the server staff." },
            {"syntax", "The right syntax is : /tban (/bann) Name Time Reason" },
            {"banned", "You have been banned by {0}. Reason: {1}" },
            {"valid_time", "You need to specify time in seconds." }
        };

        protected override void Load() {
            Instance = this;
            TellBannedPlayersNumber();
        }
    }
}