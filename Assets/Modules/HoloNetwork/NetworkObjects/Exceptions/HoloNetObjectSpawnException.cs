using System;

namespace HoloNetwork.NetworkObjects.Exceptions {

  public class HoloNetObjectSpawnException : Exception {

    public HoloNetObjectSpawnException(string message)
      : base(message) { }

    public HoloNetObjectSpawnException(string message, Exception inner)
      : base(message, inner) { }

  }

}