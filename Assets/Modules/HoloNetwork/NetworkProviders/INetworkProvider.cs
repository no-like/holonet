using System;
using System.Collections.Generic;
using HoloNetwork.Helper;
using HoloNetwork.Helper.AsyncOperations;
using HoloNetwork.Messaging;
using HoloNetwork.Messaging.Implementations;
using HoloNetwork.Players;
using HoloNetwork.RoomsManagement;

namespace HoloNetwork.NetworkProviders {

  public interface INetworkProvider {

    bool IsConnected { get; }
    bool IsInRoom { get; }
    bool IsServer { get; }

    void Init(string version);

    double GetServerTime();

    #region Network

    AsyncOp Connect(Action<AsyncOp> callback = null);
    AsyncOp Disconnect(Action<AsyncOp> callback = null);

    #endregion

    #region Room Management

    RoomListAsyncOp GetRoomList(string roomType, Action<RoomListAsyncOp> callback = null);
    AsyncOp JoinRoom(string id, Action<AsyncOp> callback = null);
    AsyncOp CreateAndJoinRoom(RoomSettings settings, Action<AsyncOp> callback = null);
    AsyncOp LeaveRoom(Action<AsyncOp> callback = null);
    void CloseCurrentRoom();
    void OpenCurrentRoom();

    #endregion

    #region Messaging

    void SendMessage(HoloNetMessage message, DestinationGroup group, bool isReliable);
    void SendMessage(HoloNetMessage message, HoloNetPlayer player, bool isReliable);

    #endregion

    #region Players

    IEnumerable<HoloNetPlayer> GetPlayers();

    #endregion

  }

}