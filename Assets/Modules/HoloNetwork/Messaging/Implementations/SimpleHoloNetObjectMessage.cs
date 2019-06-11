using System;
using HoloNetwork.ObjectPools;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class SimpleHoloNetObjectMessage<T> : HoloNetObjectMessage where T : IPoolObject {

    public static T Create() {
      return HoloNetAppModule.instance.objectPool.Pop<T>();
    }

  }

}