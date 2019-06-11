using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using HoloNetwork.Messaging.Implementations.ProviderMessages;
using HoloNetwork.Players;
using Photon.Pun;
using Photon.Realtime;

namespace HoloNetwork.NetworkProviders.Photon {

  public partial class PhotonNetworkProvider {

    public IEnumerable<HoloNetPlayer> GetPlayers() {
      return PhotonNetwork.CurrentRoom.Players.Values.Select(pp => new PhotonHoloNetPlayer(pp));
    }

    public void OnPlayerEnteredRoom(Player newPlayer) {
      var holonetPlayer = new PhotonHoloNetPlayer(newPlayer);
      HoloNetAppModule.instance.stateSynchronizer.SendStateToNewPlayer(holonetPlayer);
      HoloNetAppModule.instance.players.OnPlayerConnectedToRoom(holonetPlayer);
    }

    public void OnPlayerLeftRoom(Player otherPlayer) {
      var leavingPlayer = HoloNetAppModule.instance.players.Players.FirstOrDefault(hnp =>
        ((PhotonHoloNetPlayer) hnp).photonPlayer == otherPlayer);
      HoloNetAppModule.instance.players.OnPlayerDisconnectedFromRoom(leavingPlayer);
    }

    public void OnMasterClientSwitched(Player newMasterClient) {
      var newMasterClientPlayer = HoloNetAppModule.instance.players.Players.Select(pl => pl as PhotonHoloNetPlayer)
        .FirstOrDefault(pl => pl.photonPlayer == newMasterClient);
      HoloNetAppModule.instance.messenger.Publish(
        NetServerSwitchedMessage.Create(newMasterClientPlayer));
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }

  }

}