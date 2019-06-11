using System;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class ChangeObjectsOwnershipByOffsetMessage : HoloNetGlobalMessage {

    public int newOwnerId;
    public int offset;
    
    public static ChangeObjectsOwnershipByOffsetMessage Create(int newOwnerId, int offset) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<ChangeObjectsOwnershipByOffsetMessage>();
      obj.newOwnerId = newOwnerId;
      obj.offset = offset;
      return obj;
    }
  }

}