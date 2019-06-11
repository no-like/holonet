using System;
using System.Runtime.Serialization;
using System.Text;
using HoloNetwork.Players;
using Photon.Realtime;

namespace HoloNetwork.NetworkProviders.Photon {

  public class PhotonHoloNetPlayer : HoloNetPlayer {

    public Player photonPlayer { get; }
    public override bool isLocal => photonPlayer.IsLocal;
    public override bool isServer => photonPlayer.IsMasterClient;

    public PhotonHoloNetPlayer(Player player) : base(player.UserId) {
      photonPlayer = player;
      uniqueId = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(photonPlayer.UserId), 0);
    }

  }

  public class PhotonHoloNetPlayerSerializationRule : ISerializationSurrogate {

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
      var player = (PhotonHoloNetPlayer) obj;
      info.AddValue("uniqueId", player.actorId);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
      ISurrogateSelector selector) {
      return HoloNetPlayer.FindPlayer(info.GetInt32("uniqueId"));
    }

  }

}