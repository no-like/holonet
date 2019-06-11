using System;
using HoloNetwork.NetworkObjects;
using HoloNetwork.ObjectPools;

namespace HoloNetwork.TickSynchronization {

  [Serializable]
  public class HoloNetObjectTickState : IPoolObject {

    public HoloNetObjectId oid;

    public HoloNetObjectComponentTickState[] componentStates;

    public bool InUse { get; set; }

  }

}