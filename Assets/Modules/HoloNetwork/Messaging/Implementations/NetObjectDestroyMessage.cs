using System;
using HoloNetwork.NetworkObjects;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class NetObjectDestroyMessage : HoloNetGlobalMessage {

    public HoloNetObjectId oid;

    public static NetObjectDestroyMessage Create(HoloNetObjectId oid) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetObjectDestroyMessage>();
      obj.oid = oid;
      return obj;
    }

  }

}