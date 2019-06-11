using HoloNetwork.Players;

namespace HoloNetwork.Messaging.Implementations.ProviderMessages {

  public class NetPlayerConnectedMessage : HoloNetGlobalMessage {

    public HoloNetPlayer player;

    public static NetPlayerConnectedMessage Create(HoloNetPlayer newPlayer) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetPlayerConnectedMessage>();
      obj.player = newPlayer;
      return obj;
    }

  }

}