using System;

namespace HoloNetwork.Attributes {

  public class NetObjectMessageHandlerAttribute : Attribute {

    public Type messageType;

    public NetObjectMessageHandlerAttribute(Type messageType) {
      this.messageType = messageType;
    }

  }

  public class NetGlobalMessageHandlerAttribute : Attribute {

    public Type messageType;

    public NetGlobalMessageHandlerAttribute(Type messageType) {
      this.messageType = messageType;
    }

  }

}