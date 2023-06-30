﻿using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using System.Collections.Generic;
using Steamworks;
using System;
using SDG.Unturned;

namespace BanSystemUnturned {
    public class BanCommand : IRocketCommand {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "tban";

        public string Help => string.Empty;

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string>() { "bann" };

        public List<string> Permissions => new List<string>() { "seekers.ban" };

        public void SayToPlayerTranslation(UnturnedPlayer player, string translationkey) {
            UnturnedChat.Say(player, Plugin.Instance.Translations.Instance[translationkey], Color.cyan);
        }

        public void SayToPlayerTranslation(UnturnedPlayer player, string translationkey, params string[] args) {
            UnturnedChat.Say(player, Plugin.Instance.Translations.Instance.Translate(translationkey, args), Color.cyan);
        }

        public void SayToPlayer(UnturnedPlayer player, string message) {
            UnturnedChat.Say(player, message, Color.cyan);
        }

        public void AddToBanList(CSteamID playerId, CSteamID caller, uint banTime, string reason, DateTime time) {
            Plugin.Instance.Configuration.Instance.BannedPlayers.Add(
                new BannedPlayer((ulong)playerId, time, banTime, (ulong)caller, reason));
            Plugin.Instance.Configuration.Save();
        }
        public void TryBanPlayer(UnturnedPlayer caller, UnturnedPlayer victim, string reason, uint time) {
            bool victimIsStaff = Plugin.Instance.Configuration.Instance.
                StaffPlayers.Exists(pl => pl.playerId == ((ulong)victim.CSteamID));

            if (victimIsStaff) {
                SayToPlayerTranslation(caller, "no_ban", victim.CharacterName);
                return;
            }

            DateTime date = DateTime.Now;
            AddToBanList(victim.CSteamID, caller.CSteamID, time, reason, date);


            uint days = time / 86400;
            uint minutes = (time % 86400) / 3600;

            victim.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            EffectManager.sendUIEffect(14883, 1, true);
            EffectManager.sendUIEffectText(1, victim.CSteamID, true, "CallerName", caller.CharacterName);
            EffectManager.sendUIEffectText(1, victim.CSteamID, true, "Reason", reason);
            EffectManager.sendUIEffectText(1, victim.CSteamID, true, "BanTime", $"{days}d {minutes}m");
            EffectManager.sendUIEffectImageURL(1, victim.CSteamID, true, "CallerAvatar", caller.SteamProfile.AvatarIcon.AbsolutePath);
            //victim.Kick(Plugin.Instance.Translations.Instance.Translate("banned", caller.DisplayName, reason));
        }

        public void Execute(IRocketPlayer caller, string[] command) {
            var player = (UnturnedPlayer)caller;
            int argumentsCount = command.Length;

            switch (argumentsCount) {
                case 0:
                    SayToPlayerTranslation(player, "syntax");
                    break;
                case 1:
                    SayToPlayerTranslation(player, "one_argument");
                    break;
                case 2:
                    SayToPlayerTranslation(player, "two_arguments");
                    break;
                case 3:
                    var victim = UnturnedPlayer.FromName(command[0]);
                    uint time; 
                    bool parsed = uint.TryParse(command[1], out time);

                    if (victim == null) {
                        SayToPlayerTranslation(player, "no_player", command[0]);
                        return;
                    }
                    if (!parsed) {
                        SayToPlayerTranslation(player, "valid_time");
                        return;
                    }
                    TryBanPlayer(player, victim, command[2], time);
                    break;
            }
        }
    }
}
