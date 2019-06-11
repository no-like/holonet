using HoloNetwork.Players;

namespace HoloNetwork.Messaging.Implementations.ProviderMessages {

  public class NetServerSwitchedMessage : HoloNetGlobalMessage {

    public HoloNetPlayer newPlayerServer;

    public static NetServerSwitchedMessage Create(HoloNetPlayer newPlayer) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetServerSwitchedMessage>();
      obj.newPlayerServer = newPlayer;
      return obj;
    }

  }

}