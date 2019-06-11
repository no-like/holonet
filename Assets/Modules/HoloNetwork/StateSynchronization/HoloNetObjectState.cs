using System;
using HoloNetwork.NetworkObjects;
using HoloNetwork.ObjectPools;

namespace HoloNetwork.StateSynchronization {

  [Serializable]
  public class HoloNetObjectState : IPoolObject {

    public HoloNetObjectId oid;
    public int pid;
    public int ownerId;

    public SerializibleNetObjectState[] componentStates;

    public bool InUse { get; set; }

  }

}