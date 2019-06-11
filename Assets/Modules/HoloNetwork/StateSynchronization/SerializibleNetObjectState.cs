using System;
using HoloNetwork.ObjectPools;

namespace HoloNetwork.StateSynchronization {

  // TODO rename to componentState
  [Serializable]
  public abstract class SerializibleNetObjectState {

    public int componentId;

    public bool InUse { get; set; }

  }

}