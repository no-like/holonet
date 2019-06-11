namespace HoloNetwork.Messaging.Implementations.ProviderMessages {

  public class NetDisconnectMessage : HoloNetGlobalMessage {

    private string cause;

    public static NetDisconnectMessage Create(string cause) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetDisconnectMessage>();
      obj.cause = cause;
      return obj;
    }

  }

}