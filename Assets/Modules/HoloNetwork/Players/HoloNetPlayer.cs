using System.Runtime.Serialization;

namespace HoloNetwork.Players {

  public class HoloNetPlayer {

    /// <summary>
    /// Player id in room
    /// </summary>
    public int actorId;
    /// <summary>
    /// Player unique id in system
    /// </summary>
    public uint uniqueId;


    public virtual bool isLocal { get; }
    public virtual bool isServer { get; }

    public static HoloNetPlayer Local => HoloNetAppModule.instance.players.Local;
    public static HoloNetPlayer Server => HoloNetAppModule.instance.players.Server;

    public HoloNetPlayer(string userId) {
      actorId = userId.GetHashCode();
    }

    public static HoloNetPlayer FindPlayer(int actorId) {
      return HoloNetAppModule.instance.players.GetPlayerByActorId(actorId);
    }

    public static HoloNetPlayer FindPlayerByUniqueId(uint uniqueId) {
      return HoloNetAppModule.instance.players.GetPlayerByUniqueId(uniqueId);
    }

    protected bool Equals(HoloNetPlayer other) {
      return uniqueId == other.uniqueId;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((HoloNetPlayer) obj);
    }

    public override int GetHashCode() {
      return (int) uniqueId;
    }

    public override string ToString() {
      return $"HolonetPlayer: {actorId} {uniqueId}";
    }

  }

  public class HoloNetPlayerSerializationRule : ISerializationSurrogate {

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
      var player = (HoloNetPlayer) obj;
      info.AddValue("uniqueId", player.uniqueId);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
      ISurrogateSelector selector) {
      return HoloNetPlayer.FindPlayer(info.GetInt32("uniqueId"));
    }

  }

}