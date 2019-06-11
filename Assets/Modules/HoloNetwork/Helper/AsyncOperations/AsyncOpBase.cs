using System;

namespace HoloNetwork.Helper.AsyncOperations {

  public class AsyncOpBase<TC> where TC : AsyncOpBase<TC> {

    public bool isDone;
    public bool hasError => error != HoloNetError.NONE;
    public HoloNetError error;
    public string debugMessage;
    public Action<TC> callback;

    public AsyncOpBase() {
      isDone = false;
      error = HoloNetError.NONE;
    }

    public void InvokeCallback() {
      callback?.Invoke((TC) this);
    }

  }

}