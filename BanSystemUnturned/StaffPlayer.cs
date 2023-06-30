using Steamworks;
using System.Xml.Serialization;

namespace BanSystemUnturned {
    public class StaffPlayer {

        [XmlAttribute] public ulong playerId;

        public StaffPlayer() {

        }

        public StaffPlayer(ulong playerID) {
            this.playerId = playerID;
        }
    }
}
