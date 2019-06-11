using System.Collections.Generic;
using System.Linq;
using HoloNetwork.Messaging.Implementations.ProviderMessages;

namespace HoloNetwork.Players {

  public class PlayersManager {

    private HoloNetPlayer _localPlayerCache;

    public HoloNetPlayer Local {
      get {
        if (_localPlayerCache == null) UpdatePlayersCache();
        return _localPlayerCache;
      }
    }

    private HoloNetPlayer _serverPlayerCache;

    public HoloNetPlayer Server {
      get {
        if (_serverPlayerCache == null) UpdatePlayersCache();
        return _serverPlayerCache;
      }
    }

    public List<HoloNetPlayer> Players { get; }

    public PlayersManager() {
      Players = new List<HoloNetPlayer>();
    }

    public HoloNetPlayer GetPlayerByActorId(int actorId) {
      return Players.FirstOrDefault(item => item.actorId == actorId);
    }

    public HoloNetPlayer GetPlayerByUniqueId(uint uniqueId) {
      return Players.FirstOrDefault(item => item.uniqueId == uniqueId);
    }


    public void CleanPlayers() {
      Players.Clear();
    }

    public void RegisterPlayer(HoloNetPlayer player) {
      Players.Add(player);
    }

    public void OnPlayerConnectedToRoom(HoloNetPlayer newPlayer) {
      RegisterPlayer(newPlayer);
      HoloNetAppModule.instance.messenger.Publish(NetPlayerConnectedMessage.Create(newPlayer));
      UpdatePlayersCache();
    }

    public void OnPlayerDisconnectedFromRoom(HoloNetPlayer disconnectedPlayer) {
      HoloNetAppModule.instance.messenger.Publish(NetPlayerDisconnectedMessage.Create(disconnectedPlayer));
      Players.Remove(disconnectedPlayer);
      UpdatePlayersCache();
    }

    private void UpdatePlayersCache() {
      _serverPlayerCache = Players.FirstOrDefault(item => item.isServer);
      _localPlayerCache = Players.FirstOrDefault(item => item.isLocal);
    }

  }

}