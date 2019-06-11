namespace HoloNetwork.RoomsManagement {

  public class RoomsManager {

    public bool isInRoom => HoloNetAppModule.instance.provider.IsInRoom;
    public HoloNetRoom currentRoom;

    public void CloseCurrentRoom() {
      if (isInRoom) HoloNetAppModule.instance.provider.CloseCurrentRoom();
    }

    public void OpenCurrentRoom() {
      if (isInRoom) HoloNetAppModule.instance.provider.OpenCurrentRoom();
    }

    public void SetCurrentRoom(HoloNetRoom newRoom) {
      currentRoom = newRoom;
    }

    public void ClearCurrentRoom() {
      currentRoom = null;
    }

  }

}