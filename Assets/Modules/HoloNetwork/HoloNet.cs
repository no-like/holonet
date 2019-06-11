using System;
using System.Collections;
using System.Runtime.Serialization;
using HoloNetwork.Helper.AsyncOperations;
using HoloNetwork.Messaging;
using HoloNetwork.Messaging.Implementations;
using HoloNetwork.NetworkObjects;
using HoloNetwork.ObjectPools;
using HoloNetwork.Players;
using HoloNetwork.RoomsManagement;
using Misc;
using UnityEngine;

namespace HoloNetwork {

  public class HoloNet {

    #region Network

    public static double serverTime => HoloNetAppModule.instance.provider.GetServerTime();
    public static bool isServerClient => HoloNetAppModule.instance.provider.IsServer;

    public static AsyncOp Connect(Action<AsyncOp> callback = null) {
      return HoloNetAppModule.instance.provider.Connect(callback);
    }

    public static AsyncOp Disconnect(Action<AsyncOp> callback = null) {
      return HoloNetAppModule.instance.provider.Disconnect(callback);
    }

    public static bool IsConnected() {
      return HoloNetAppModule.instance.provider.IsConnected;
    }

    #endregion

    #region Messages

    public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> handler)
      where TMessage : HoloNetGlobalMessage {
      return HoloNetAppModule.instance.messenger.Subscribe<TMessage>(handler);
    }

    public static void Unsubscribe<TMessage>(TinyMessageSubscriptionToken token)
      where TMessage : HoloNetGlobalMessage {
      HoloNetAppModule.instance.messenger.Unsubscribe<TMessage>(token);
    }

    public static void SendReliable(HoloNetGlobalMessage message, DestinationGroup group = DestinationGroup.All) {
      HoloNetAppModule.instance.messenger.SendMessage(message, group, true);
    }

    public static void SendUnreliable(HoloNetGlobalMessage message, DestinationGroup group = DestinationGroup.All) {
      HoloNetAppModule.instance.messenger.SendMessage(message, group, false);
    }

    public static void SendReliable(HoloNetGlobalMessage message, HoloNetPlayer player) {
      HoloNetAppModule.instance.messenger.SendMessage(message, player, true);
    }

    public static void SendUnreliable(HoloNetGlobalMessage message, HoloNetPlayer player) {
      HoloNetAppModule.instance.messenger.SendMessage(message, player, false);
    }

    public static void RegisterMutedMessageOverride<T>() where T : HoloNetMessage {
      HoloNetAppModule.instance.messenger.RegisterMutedMessageOverride<T>();
    }

    public static void PauseMessaging() {
      HoloNetAppModule.instance.messenger.Pause();
    }

    public static void ResumeMessaging() {
      HoloNetAppModule.instance.messenger.Resume();
    }

    public static void PauseTickSync() {
      HoloNetAppModule.instance.tickSynchronizer.Pause();
    }

    public static void ResumeTickSync() {
      HoloNetAppModule.instance.tickSynchronizer.Resume();
    }

    #endregion

    #region Objects Management

    public static HoloNetObject GetObject(HoloNetObjectId id) {
      return HoloNetAppModule.instance.objectsManager.GetObject(id);
    }

    public static T GetObject<T>(HoloNetObjectId id) where T : MonoBehaviour {
      var hno = GetObject(id);
      return hno == null ? null : hno.GetComponentInChildren<T>();
    }

    public static HoloNetObject SpawnNetObject(GameObject go) {
      return HoloNetAppModule.instance.objectsManager.Spawn(go);
    }

    public static HoloNetObject SpawnNetObject(MonoBehaviour comp) {
      return HoloNetAppModule.instance.objectsManager.Spawn(comp);
    }

    public static HoloNetObject SpawnNetObject(MonoBehaviour comp, Vector3 position, Quaternion rotation) {
      return HoloNetAppModule.instance.objectsManager.Spawn(comp, position, rotation);
    }

    public static HoloNetObject SpawnNetObject(GameObject go, Vector3 position, Quaternion rotation) {
      return HoloNetAppModule.instance.objectsManager.Spawn(go, position, rotation);
    }

    public static void DestroyNetObject(HoloNetObject obj) {
      HoloNetAppModule.instance.objectsManager.DestroyNetObject(obj);
    }

    /// <summary>
    /// Destroys net object without checking it`s ownership.
    /// Use when you need to cleanup everything player left after disconnecting.
    /// </summary>
    /// <param name="obj">Object to destroy</param>
    /// <param name="delay"></param>
    public static void ForceDestroyNetObject(HoloNetObject obj) {
      HoloNetAppModule.instance.objectsManager.ForceDestroyNetObject(obj);
    }

    public static void RegisterSceneObjects() {
      HoloNetAppModule.instance.objectsManager.RegisterSceneObjects();
    }

    public static IEnumerator RegisterSceneObjectsContinious() {
      yield return HoloNetAppModule.instance.objectsManager.RegisterSceneObjectsContinious();
    }

    public static void CleanUpDestroyedObjects() {
      HoloNetAppModule.instance.objectsManager.CleanUpDestroyedObjects();
    }

    #endregion

    #region Rooms

    public static bool isInRoom => HoloNetAppModule.instance.rooms.isInRoom;

    public static HoloNetRoom currentRoom => HoloNetAppModule.instance.rooms.currentRoom;

    public static RoomListAsyncOp GetRoomList(string roomType, Action<RoomListAsyncOp> callback = null) {
      return HoloNetAppModule.instance.provider.GetRoomList(roomType, callback);
    }

    public static AsyncOp JoinRoom(HoloNetRoom room, Action<AsyncOp> callback = null) {
      return HoloNetAppModule.instance.provider.JoinRoom(room.id, callback);
    }

    public static AsyncOp CreateRoom(RoomSettings settings, Action<AsyncOp> callback = null) {
      return HoloNetAppModule.instance.provider.CreateAndJoinRoom(settings, callback);
    }

    public static AsyncOp LeaveRoom(Action<AsyncOp> callback = null) {
      return HoloNetAppModule.instance.provider.LeaveRoom(callback);
    }

    public static void CloseCurrentRoom() {
      HoloNetAppModule.instance.rooms.CloseCurrentRoom();
    }

    public static void OpenCurrentRoom() {
      HoloNetAppModule.instance.rooms.OpenCurrentRoom();
    }

    #endregion

    #region Reconnect

    public static AsyncOp WaitAndApplySyncState(Action<AsyncOp> callback = null) {
      return HoloNetAppModule.instance.stateSynchronizer.WaitAndApplySyncState(callback);
    }

    #endregion

    public static void RegisterSerialiationRule<T>(ISerializationSurrogate serializationRule) {
      HoloNetAppModule.instance.serializer.RegisterSerializationRule<T>(serializationRule);
    }

    public static ObjectPool GetObjectPool() {
      return HoloNetAppModule.instance.objectPool;
    }

  }

}