using System;
using System.Collections.Generic;
using HoloNetwork.Messaging.Implementations;

namespace HoloNetwork.TickSynchronization {

  [Serializable]
  public class TickSyncMessage : HoloNetGlobalMessage {

    public List<HoloNetObjectTickState> objectTickStates;

    public static TickSyncMessage Create(List<HoloNetObjectTickState> objectTickStates) {
//      var obj = HoloNetAppModule.instance.objectPool.Pop<TickSyncMessage>();
      var obj = new TickSyncMessage();
      obj.objectTickStates = objectTickStates;
      return obj;
    }

  }

}