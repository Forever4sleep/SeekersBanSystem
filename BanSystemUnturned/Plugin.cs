using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Steam;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BanSystemUnturned {
    public class Plugin : RocketPlugin<Config> {

        public const string EndButtonName = "End";
        public const string SendMessageButton = "SendMessage";
        public static Plugin Instance;

        Dictionary<Player, string> bannedPlayersOnTheServer = new Dictionary<Player, string>();
        private void TellBannedPlayersNumber() {
            Rocket.Core.Logging.Logger.Log($"There are {this.Configuration.Instance.BannedPlayers.Count} players on this server.");
        }

        public override TranslationList DefaultTranslations => new TranslationList() {
            {"one_argument", "You need to specify time and reason." },
            {"two_arguments", "You need to specify the ban reason." },
            {"no_player", "There is no such player as {0} present on the server right now." },
            {"no_ban", "You cannot ban {0} since they belong to the server staff." },
            {"syntax", "The right syntax is : /tban (/bann) Name Time Reason" },
            {"banned", "You have been banned by {0}. Reason: {1}" },
            {"valid_time", "Error occured. You need to specify time in seconds. Or the value was too big." },
            {"already_banned", "You cannot ban the same player twice." }
        };

        protected override void Load() {
            Instance = this;
            TellBannedPlayersNumber();
            EffectManager.onEffectButtonClicked += Events_EffectButtonClicked;
            EffectManager.onEffectTextCommitted += Events_TextCommited;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player) {
            bannedPlayersOnTheServer.Remove(player.Player);
        }

        private void Events_TextCommited(Player player, string buttonName, string text) {
            if (buttonName == "InputField" && text != string.Empty) {
                var data = Configuration.Instance.BannedPlayers.FirstOrDefault(x => x.playerId == ((ulong)player.channel.owner.playerID.steamID));
                if (data != null && data.numberOfMessages < Configuration.Instance.maxMessagesToAdmin) {
                    bannedPlayersOnTheServer[player] = text;
                }
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer victim) {
            BannedPlayer data = Configuration.Instance.BannedPlayers.FirstOrDefault(x => x.playerId == ((ulong)victim.CSteamID));
            if (data == null) return;
            bannedPlayersOnTheServer.Add(victim.Player, string.Empty);
            if ((ulong)(DateTime.Now - data.banDate).Seconds >= data.banTime) {
                Configuration.Instance.BannedPlayers.Remove(data);
                return;
            }

            ulong days = data.banTime / 86400;
            ulong hours = (data.banTime % 86400) / 3600;
            ulong minutes = (data.banTime % 3600) / 60;
            victim.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            EffectManager.sendUIEffect(14883, 1, true);
            EffectManager.sendUIEffectText(1, victim.CSteamID, true, "CallerName", data.bannedByName);
            EffectManager.sendUIEffectText(1, victim.CSteamID, true, "Reason", data.reason);
            EffectManager.sendUIEffectText(1, victim.CSteamID, true, "BanTime", $"{days}d {hours}h {minutes}m");
                
            Profile getProfile = new Profile(data.bannedBy);
            EffectManager.sendUIEffectImageURL(1, victim.CSteamID, true, "CallerAvatar", getProfile.AvatarFull.ToString());

            Configuration.Save();
        }

        protected override void Unload() {
            base.Unload();
            EffectManager.onEffectButtonClicked -= Events_EffectButtonClicked;
            EffectManager.onEffectTextCommitted -= Events_TextCommited;
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
        }

        private void Events_EffectButtonClicked(Player player, string buttonName) {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            BannedPlayer data = Configuration.Instance.BannedPlayers.FirstOrDefault(pl => pl.playerId == (ulong)uPlayer.CSteamID);
            if (buttonName == EndButtonName) {
                if (data == null) {
                    uPlayer.Kick(string.Empty);
                    return;
                }
                uPlayer.Kick(DefaultTranslations.Translate("banned", data.bannedByName, data.reason));
            }
            if (buttonName == SendMessageButton) {
                string text = bannedPlayersOnTheServer[player];
                if (text != string.Empty && data.numberOfMessages < Configuration.Instance.maxMessagesToAdmin) {
                    data.numberOfMessages++;
                    Configuration.Save();
                }
            }
        }
    }
}