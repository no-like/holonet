namespace HoloNetwork.Messaging.Implementations.ProviderMessages {

  /// <summary>
  /// Sent to local player whe he leaves room
  /// </summary>
  public class NetRoomLeftMessage : HoloNetGlobalMessage {

    public static NetRoomLeftMessage Create() {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetRoomLeftMessage>();
      return obj;
    }

  }

}