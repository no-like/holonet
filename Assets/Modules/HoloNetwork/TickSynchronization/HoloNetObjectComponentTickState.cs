using System;
using HoloNetwork.ObjectPools;

namespace HoloNetwork.TickSynchronization {

  [Serializable]
  public class HoloNetObjectComponentTickState : IPoolObject {

    public int componentId;

    public bool InUse { get; set; }

  }

}