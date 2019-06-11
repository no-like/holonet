namespace HoloNetwork.StateSynchronization {

  public interface INetStateSyncable {

    SerializibleNetObjectState GetSyncState();
    void ApplySyncState(SerializibleNetObjectState state);

  }

}