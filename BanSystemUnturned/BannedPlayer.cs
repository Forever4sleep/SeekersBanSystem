using Steamworks;
using System;
using System.Xml.Serialization;

namespace BanSystemUnturned {
    public class BannedPlayer {
        
        [XmlAttribute] public ulong playerId;
        [XmlAttribute] public DateTime banDate;
        public string reason;
        [XmlAttribute] public ulong banTime;
        [XmlAttribute] public ulong bannedBy;
        [XmlAttribute] public string bannedByName;
        [XmlAttribute] public byte numberOfMessages;

        public BannedPlayer() { }

        public BannedPlayer(ulong playerId, DateTime banDate, ulong banTime, ulong bannedBy, string reason, string bannedByName, byte numberOfMessages) {
            this.playerId = playerId;
            this.banDate = banDate;
            this.banTime = banTime;
            this.bannedBy = bannedBy;
            this.reason = reason;
            this.bannedByName = bannedByName;
            this.numberOfMessages = numberOfMessages;
        }
    }
}
