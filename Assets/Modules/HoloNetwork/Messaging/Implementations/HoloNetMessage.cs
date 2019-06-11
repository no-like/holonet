using System;
using HoloNetwork.ObjectPools;
using Misc;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public abstract class HoloNetMessage : ITinyMessage, IPoolObject {

    public abstract HoloNetMessageType messageType { get; }

    public static T Pop<T>() where T : HoloNetMessage {
      return HoloNet.GetObjectPool().Pop<T>();
    }

    public double sendTime;

    public object Sender { get; }

    public virtual bool InUse { get; set; }

  }

  public enum HoloNetMessageType {

    GLOBAL,
    OBJECT

  }

}