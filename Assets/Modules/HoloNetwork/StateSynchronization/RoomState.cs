using System;
using System.Collections.Generic;

namespace HoloNetwork.StateSynchronization {

  [Serializable]
  public class RoomState {

    public double creationTime;
    public List<HoloNetObjectState> objectStates;

  }

}