namespace HoloNetwork.TickSynchronization {

  public interface INetTickSyncable {

    HoloNetObjectComponentTickState GetTickState();
    void ApplyTickState(HoloNetObjectComponentTickState state);

  }

}