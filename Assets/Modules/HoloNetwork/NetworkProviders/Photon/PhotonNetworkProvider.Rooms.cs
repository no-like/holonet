using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using HoloNetwork.Helper;
using HoloNetwork.Helper.AsyncOperations;
using HoloNetwork.RoomsManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using LobbyType = Photon.Realtime.LobbyType;

namespace HoloNetwork.NetworkProviders.Photon {

  public partial class PhotonNetworkProvider {

    public bool IsInRoom => PhotonNetwork.InRoom;

    private RoomListAsyncOp _getRoomsAsyncOp;
    private AsyncOp _joinRoomOp;
    private AsyncOp _createAndJoinRoomOp;
    private AsyncOp _leaveRoomOp;

    private bool canExecuteOperation => _getRoomsAsyncOp == null && _joinRoomOp == null &&
                                        _createAndJoinRoomOp == null && _leaveRoomOp == null;

    #region Room list

    public RoomListAsyncOp GetRoomList(string roomType, Action<RoomListAsyncOp> callback = null) {
      Debug.Log($"[HOLONET] ROOMS - GetRoomList for version {PhotonNetwork.AppVersion}");
      if (!canExecuteOperation) return CreateFail(HoloNetError.ANOTHER_ROOM_OPERATION_IN_PRORESS, callback);
      if (!PhotonNetwork.GetCustomRoomList(new TypedLobby("Default", LobbyType.SqlLobby),
        $"C0 = '{roomType}'")) {
        return CreateFail(HoloNetError.PROVIDER_NOT_READY, callback);
      }

      _getRoomsAsyncOp = new RoomListAsyncOp();
      _getRoomsAsyncOp.callback = callback;
      _getRoomsAsyncOp.rooms = new List<HoloNetRoom>();
      return _getRoomsAsyncOp;
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList) {
      if (_getRoomsAsyncOp == null) return;
      Debug.Log($"[HOLONET] ROOMS - OnRoomListUpdate");
      foreach (var roomInfo in roomList) {
        Debug.Log($"[HOLONET] ROOMS - OnRoomListUpdate {roomInfo.Name} {roomInfo.MaxPlayers}");
      }

      foreach (var photonRoomInfo in roomList) {
        _getRoomsAsyncOp.rooms.Add(ConvertToHolonetRoom(photonRoomInfo));
      }

      _getRoomsAsyncOp.isDone = true;
      _getRoomsAsyncOp.InvokeCallback();
      _getRoomsAsyncOp = null;
    }

    #endregion

    #region Join Room

    public AsyncOp JoinRoom(string id, Action<AsyncOp> callback = null) {
      Debug.Log($"[HOLONET] ROOMS - JoinRoom");
      if (!canExecuteOperation) return CreateFail(HoloNetError.ANOTHER_ROOM_OPERATION_IN_PRORESS, callback);
      if (!PhotonNetwork.JoinRoom(id)) {
        return CreateFail(HoloNetError.PROVIDER_NOT_READY, callback);
      }

      _joinRoomOp = new AsyncOp();
      _joinRoomOp.callback = callback;
      return _joinRoomOp;
    }

    public void OnJoinedRoom() {
      if (_joinRoomOp == null && _createAndJoinRoomOp == null) return;
      Debug.Log($"[HOLONET] ROOMS - OnJoinedRoom");
      HoloNetAppModule.instance.OnConnectedToRoom();

      if (_joinRoomOp != null) {
        _joinRoomOp.isDone = true;
        _joinRoomOp.InvokeCallback();
        _joinRoomOp = null;
      } else if (_createAndJoinRoomOp != null) {
        _createAndJoinRoomOp.isDone = true;
        _createAndJoinRoomOp.InvokeCallback();
        _createAndJoinRoomOp = null;
      }

      HoloNetAppModule.instance.rooms.SetCurrentRoom(ConvertToHolonetRoom(PhotonNetwork.CurrentRoom));
    }

    public void OnJoinRoomFailed(short returnCode, string message) {
      if (_joinRoomOp == null) return;
      Debug.Log($"[HOLONET] ROOMS - OnJoinRoomFailed {message}");
      _joinRoomOp.isDone = true;
      _joinRoomOp.error = HoloNetError.JOIN_ROOM_FAILED;
      _joinRoomOp.debugMessage = $"Join Room Failed, ServerCode: {returnCode}, Message:{message}";
      _joinRoomOp.InvokeCallback();
      _joinRoomOp = null;
    }

    #endregion

    #region Create Room

    public AsyncOp CreateAndJoinRoom(RoomSettings settings, Action<AsyncOp> callback = null) {
      Debug.Log($"[HOLONET] ROOMS - CreateRoom");
      if (!canExecuteOperation) return CreateFail(HoloNetError.ANOTHER_ROOM_OPERATION_IN_PRORESS, callback);

      if (!PhotonNetwork.CreateRoom(null, ConvertToPhotonRoomOptions(settings),
        new TypedLobby("Default", LobbyType.SqlLobby))) {
        _createAndJoinRoomOp.error = HoloNetError.CREATE_ROOM_FAILED;
      }

      _createAndJoinRoomOp = new AsyncOp();
      _createAndJoinRoomOp.callback = callback;
      return _createAndJoinRoomOp;
    }

    public void OnCreateRoomFailed(short returnCode, string message) {
      if (_createAndJoinRoomOp == null) return;
      Debug.Log($"[HOLONET] ROOMS - OnCreateRoomFailed {message}");
      _createAndJoinRoomOp.isDone = true;
      _createAndJoinRoomOp.error = HoloNetError.CREATE_ROOM_FAILED;
      _createAndJoinRoomOp.debugMessage = $"Create Room Failed, ServerCode: {returnCode}, Message:{message}";
      _createAndJoinRoomOp.InvokeCallback();
      _createAndJoinRoomOp = null;
    }

    #endregion

    #region Leave Room

    public AsyncOp LeaveRoom(Action<AsyncOp> callback = null) {
      Debug.Log($"[HOLONET] ROOMS - LeaveRoom");
      if (!canExecuteOperation) return CreateFail(HoloNetError.ANOTHER_ROOM_OPERATION_IN_PRORESS, callback);


      if (!PhotonNetwork.LeaveRoom(false)) {
        return CreateFail(HoloNetError.PROVIDER_NOT_READY, callback);
      }

      _leaveRoomOp = new AsyncOp();
      _leaveRoomOp.callback = callback;
      return _leaveRoomOp;
    }

    #endregion

    public void CloseCurrentRoom() {
      PhotonNetwork.CurrentRoom.IsOpen = false;
      PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void OpenCurrentRoom() {
      PhotonNetwork.CurrentRoom.IsOpen = true;
      PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    #region Unused Callbacks

    public void OnLeftRoom() { }
    public void OnJoinRandomFailed(short returnCode, string message) { }
    public void OnCreatedRoom() { }

    #endregion


    private static T CreateFail<T>(HoloNetError error, Action<T> callback) where T : AsyncOpBase<T>, new() {
      var result = new T {
        isDone = true,
        error = error,
        callback = callback
      };
      result.InvokeCallback();
      return result;
    }


    private HoloNetRoom ConvertToHolonetRoom(RoomInfo photonRoomInfo) {
      var result = new HoloNetRoom();
      result.id = photonRoomInfo.Name;
      result.name = (string) photonRoomInfo.CustomProperties["N"] ?? "";
      result.roomType = (string) photonRoomInfo.CustomProperties["C0"] ?? "";

      result.isVisible = photonRoomInfo.IsVisible;
      result.isOpen = photonRoomInfo.IsOpen;

      result.playersCount = photonRoomInfo.PlayerCount;
      result.maxPlayers = photonRoomInfo.MaxPlayers;

      result.password = (string) photonRoomInfo.CustomProperties["P"] ?? "";
      return result;
    }


    private RoomOptions ConvertToPhotonRoomOptions(RoomSettings roomSetings) {
      var photonCustomRoomProperties = new Hashtable();
      photonCustomRoomProperties["C0"] = roomSetings.roomType;
      photonCustomRoomProperties["P"] = roomSetings.password;
      photonCustomRoomProperties["N"] = roomSetings.name;

      return new RoomOptions {
        PublishUserId = true,
        IsOpen = roomSetings.isOpen,
        IsVisible = roomSetings.isVisible,
        MaxPlayers = roomSetings.maxPlayers,
        CustomRoomProperties = photonCustomRoomProperties,
        CustomRoomPropertiesForLobby = new[] {"C0", "P", "N"}
      };
    }

  }

}