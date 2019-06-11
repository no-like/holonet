using System;
using AppControllers;
using HoloNetwork.Messaging.Implementations;

namespace HoloNetwork.NetworkObjects {

  [Serializable]
  public class HoloNetObjectTransferOwnershipMessage : HoloNetObjectMessage {

    public uint newOwnerUniqueId;

    public static HoloNetObjectTransferOwnershipMessage Create(uint newOwnerUniqueId) {
      var obj = AppController.instance.holonet.objectPool.Pop<HoloNetObjectTransferOwnershipMessage>();
      obj.newOwnerUniqueId = newOwnerUniqueId;
      return obj;
    }

  }

}