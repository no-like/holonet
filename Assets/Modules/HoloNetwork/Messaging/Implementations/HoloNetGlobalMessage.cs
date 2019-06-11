using System;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class HoloNetGlobalMessage : HoloNetMessage {

    public override HoloNetMessageType messageType => HoloNetMessageType.GLOBAL;

  }

}