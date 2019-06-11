using ExitGames.Client.Photon;
using HoloNetwork.Messaging;
using HoloNetwork.Messaging.Implementations;
using HoloNetwork.Players;
using Photon.Pun;
using Photon.Realtime;

namespace HoloNetwork.NetworkProviders.Photon {

  public partial class PhotonNetworkProvider {

    public void SendMessage(HoloNetMessage message, DestinationGroup group, bool isReliable) {
      SendInternal(message, isReliable,
        new RaiseEventOptions {Receivers = ConvertDestinationToPhotonReceiverEnum(group)}
      );
    }

    public void SendMessage(HoloNetMessage message, HoloNetPlayer player, bool isReliable) {
      var photonPlayer = (PhotonHoloNetPlayer) player;
      SendInternal(message, isReliable,
        new RaiseEventOptions {TargetActors = new[] {photonPlayer.photonPlayer.ActorNumber}}
      );
    }

    private void SendInternal(HoloNetMessage message, bool isReliable, RaiseEventOptions raiseEventOptions) {
      raiseEventOptions.CachingOption = EventCaching.DoNotCache;
      raiseEventOptions.Flags = WebFlags.Default;
      PhotonNetwork.RaiseEvent(
        (byte) message.messageType,
        HoloNetAppModule.instance.serializer.Serialize(message),
        raiseEventOptions,
        isReliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
    }

    public void OnEvent(EventData photonEvent) {
      if (photonEvent.Code >= 200) return;
      var message = HoloNetAppModule.instance.serializer.Deserialize<HoloNetMessage>((byte[]) photonEvent.CustomData);
      HoloNetAppModule.instance.messenger.OnMessageReceived(message);
    }

    private static ReceiverGroup ConvertDestinationToPhotonReceiverEnum(DestinationGroup group) {
      var photonGroup = ReceiverGroup.All;
      switch (group) {
        case DestinationGroup.All:
          photonGroup = ReceiverGroup.All;
          break;
        case DestinationGroup.Others:
          photonGroup = ReceiverGroup.Others;
          break;
        case DestinationGroup.Server:
          photonGroup = ReceiverGroup.MasterClient;
          break;
      }

      return photonGroup;
    }

  }

}