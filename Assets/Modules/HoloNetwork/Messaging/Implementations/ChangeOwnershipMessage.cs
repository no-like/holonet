using System;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class ChangeObjectsOwnershipMessage : HoloNetGlobalMessage {

    public int owner;
    public int newOwner;
    
    public static ChangeObjectsOwnershipMessage Create(int owner, int newOwner) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<ChangeObjectsOwnershipMessage>();
      obj.owner = owner;
      obj.newOwner = newOwner;
      return obj;
    }
  }

}