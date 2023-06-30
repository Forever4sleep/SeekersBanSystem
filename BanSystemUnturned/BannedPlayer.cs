using Steamworks;
using System;
using System.Xml.Serialization;

namespace BanSystemUnturned {
    public class BannedPlayer {
        
        [XmlAttribute] public ulong playerId;
        [XmlAttribute] public DateTime banDate;
        public string reason;
        [XmlAttribute] public uint banTime;
        [XmlAttribute] public ulong bannedBy;

        public BannedPlayer() { }

        public BannedPlayer(ulong playerId, DateTime banDate, uint banTime, ulong bannedBy, string reason) {
            this.playerId = playerId;
            this.banDate = banDate;
            this.banTime = banTime;
            this.bannedBy = bannedBy;
            this.reason = reason;
        }
    }
}
