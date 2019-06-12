using System;
using HoloNetwork.Helper;
using HoloNetwork.Helper.AsyncOperations;
using HoloNetwork.Messaging.Implementations.ProviderMessages;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace HoloNetwork.NetworkProviders.Photon {

  public partial class PhotonNetworkProvider {

    public bool IsConnected => PhotonNetwork.IsConnectedAndReady;
    private AsyncOp _connectAsyncOp;
    private AsyncOp _currentNetworkOp;

    private bool canExecuteNetworkOperations => _connectAsyncOp == null && _currentNetworkOp == null;

    #region Connection

    public AsyncOp Connect(Action<AsyncOp> callback = null) {
      Debug.Log($"[HOLONET] NETWORK - Connecting");
      if (!canExecuteNetworkOperations) return CreateFail(HoloNetError.ANOTHER_NETWORK_OPERATION_IN_PRORESS, callback);
     
      if (!PhotonNetwork.ConnectUsingSettings()) {
        return CreateFail(HoloNetError.ALREADY_CONNECTED, callback);
      }

      _connectAsyncOp = new AsyncOp();
      _connectAsyncOp.callback = callback;
      return _connectAsyncOp;
    }

    public void OnConnected() {
      PhotonNetwork.GameVersion = version;
    }

    public void OnConnectedToMaster() {
      if (_leaveRoomOp != null) {
        Debug.Log($"[HOLONET] ROOMS - OnLeftRoom");
        _leaveRoomOp.isDone = true;
        _leaveRoomOp.InvokeCallback();
        _leaveRoomOp = null;

        HoloNetAppModule.instance.stateSynchronizer.ClearRoomState();
        HoloNetAppModule.instance.messenger.Pause();
        HoloNetAppModule.instance.tickSynchronizer.Pause();

        HoloNetAppModule.instance.messenger.Publish(NetRoomLeftMessage.Create());
      }

      if (_connectAsyncOp != null) {
        Debug.Log($"[HOLONET] NETWORK - Connected");
        PhotonNetwork.JoinLobby(new TypedLobby("Default", LobbyType.SqlLobby));
      }

      HoloNetAppModule.instance.rooms.ClearCurrentRoom();
    }

    public void OnJoinedLobby() {
      if (_connectAsyncOp == null) return;
      Debug.Log($"[HOLONET] NETWORK - Connected to Lobby {PhotonNetwork.CurrentLobby}");
      if (_connectAsyncOp != null) {
        _connectAsyncOp.isDone = true;
        _connectAsyncOp.InvokeCallback();
        _connectAsyncOp = null;
      }
    }

    #endregion


    public AsyncOp Disconnect(Action<AsyncOp> callback = null) {
      Debug.Log($"[HOLONET] NETWORK - Disconnecting");
      if (_currentNetworkOp != null) {
        return CreateFail(HoloNetError.ANOTHER_NETWORK_OPERATION_IN_PRORESS, callback);
      }

      PhotonNetwork.Disconnect();
      _currentNetworkOp = new AsyncOp();
      _currentNetworkOp.callback = callback;
      return _currentNetworkOp;
    }

    #region Callbacks

    public void OnDisconnected(DisconnectCause cause) {
      if (_currentNetworkOp != null) {
        _currentNetworkOp.isDone = true;
        _currentNetworkOp.error = HoloNetError.DISCONNECTED_FROM_PROVIDER;
        _currentNetworkOp.debugMessage = $"Disconnected from provider, DisconnectCause: {cause}";
        _currentNetworkOp.InvokeCallback();
        _currentNetworkOp = null;
      }

      HoloNetAppModule.instance.messenger.Publish(NetDisconnectMessage.Create(cause.ToString()));
    }


    public void OnLeftLobby() { }

    #endregion

  }

}