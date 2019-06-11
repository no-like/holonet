using System;
using HoloNetwork.NetworkObjects;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class HoloNetObjectMessage : HoloNetMessage {

    public override HoloNetMessageType messageType => HoloNetMessageType.OBJECT;

    public HoloNetObjectId receiverId;
    
    

  }

}