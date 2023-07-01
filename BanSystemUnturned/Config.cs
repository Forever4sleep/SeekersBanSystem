using Rocket.API;
using System.Collections.Generic;

namespace BanSystemUnturned {
    public class Config : IRocketPluginConfiguration {
        public List<BannedPlayer> BannedPlayers { get; set; }

        public List<StaffPlayer> StaffPlayers { get; set; }

        public byte maxMessagesToAdmin;
        public void LoadDefaults() {
            BannedPlayers = new List<BannedPlayer>();
            StaffPlayers = new List<StaffPlayer>();
            maxMessagesToAdmin = 3;
        }
    }
}
