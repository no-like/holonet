using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace HoloNetwork.NetworkProviders.Photon {

  public partial class PhotonNetworkProvider :
    INetworkProvider,
    IConnectionCallbacks,
    IMatchmakingCallbacks,
    IInRoomCallbacks,
    ILobbyCallbacks,
    IOnEventCallback {

    private string version;

    public bool IsServer => PhotonNetwork.IsMasterClient;

    public void Init(string version) {
      this.version = version;

      //This is ugly. Blame PUN 2
      var ss = (ServerSettings) Resources.Load("PhotonServerSettings", typeof(ServerSettings));
      ss.AppSettings.AppVersion = version;

      PhotonNetwork.AddCallbackTarget(this);
      PhotonNetwork.AutomaticallySyncScene = false;
      PhotonNetwork.OfflineMode = false;
      PhotonNetwork.LogLevel = PunLogLevel.ErrorsOnly;
      PhotonNetwork.GameVersion = version;
      HoloNet.RegisterSerialiationRule<PhotonHoloNetPlayer>(new PhotonHoloNetPlayerSerializationRule());
    }

    public double GetServerTime() {
      return PhotonNetwork.Time;
    }

    #region photonCallbacks

    public void OnRegionListReceived(RegionHandler regionHandler) { }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

    public void OnCustomAuthenticationFailed(string debugMessage) { }

    public void OnFriendListUpdate(List<FriendInfo> friendList) { }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) { }

    #endregion

  }

}