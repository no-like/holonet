using HoloNetwork.Players;

namespace HoloNetwork.Messaging.Implementations.ProviderMessages {

  public class NetPlayerReconnectedMessage : HoloNetGlobalMessage {

    public HoloNetPlayer player;

    public static NetPlayerReconnectedMessage Create(HoloNetPlayer player) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetPlayerReconnectedMessage>();
      obj.player = player;
      return obj;
    }

  }

}