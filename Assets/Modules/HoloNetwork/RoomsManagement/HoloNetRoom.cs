namespace HoloNetwork.RoomsManagement {

  public class RoomSettings {

    public string name;
    public string roomType;
    public byte maxPlayers;
    public bool isVisible;
    public bool isOpen;
    public string password;

  }

  public class HoloNetRoom {

    public string id;
    public string name;
    public string roomType;

    public bool isVisible;
    public bool isOpen;

    public int maxPlayers;
    public int playersCount;

    public string password;

    public bool HasPassword => !string.IsNullOrEmpty(password);
    public bool IsFull => playersCount >= maxPlayers;

  }

}