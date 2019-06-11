using HoloNetwork.Players;

namespace HoloNetwork.Messaging.Implementations.ProviderMessages {

  public class NetPlayerDisconnectedMessage : HoloNetGlobalMessage {

    public HoloNetPlayer player;

    public static NetPlayerDisconnectedMessage Create(HoloNetPlayer player) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetPlayerDisconnectedMessage>();
      obj.player = player;
      return obj;
    }

  }

}