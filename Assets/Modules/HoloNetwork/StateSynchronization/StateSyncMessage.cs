using System;
using HoloNetwork.Messaging.Implementations;

namespace HoloNetwork.StateSynchronization {

  [Serializable]
  public class StateSyncMessage : HoloNetGlobalMessage {

    public RoomState roomState;

    public static StateSyncMessage Create(RoomState roomState) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<StateSyncMessage>();
      obj.roomState = roomState;
      return obj;
    }

  }

}