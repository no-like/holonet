using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppControllers;
using HoloNetwork.Helper;
using HoloNetwork.Helper.AsyncOperations;
using HoloNetwork.NetworkObjects;
using HoloNetwork.Players;
using UnityEngine;

namespace HoloNetwork.StateSynchronization {

  public class StateSynchronizer {

    public RoomState roomStateToApply { get; private set; }

    public void Init() {
      HoloNetAppModule.instance.messenger.Subscribe<StateSyncMessage>(OnStateReceived);
      HoloNetAppModule.instance.messenger.RegisterMutedMessageOverride<StateSyncMessage>();
    }

    public void ClearRoomState() {
      roomStateToApply = null;
    }

    public bool HasStateToApply() {
      return roomStateToApply != null;
    }

    private RoomState PackAllStates() {
      var result = new RoomState();
      var objects = HoloNetAppModule.instance.objectsManager.GetAll();

      result.objectStates = new List<HoloNetObjectState>();
      foreach (var item in objects) {
        result.objectStates.Add(item.GetState());
      }

      result.creationTime = HoloNetAppModule.instance.provider.GetServerTime();
      return result;
    }

    public void SendStateToNewPlayer(HoloNetPlayer holonetPlayer) {
      if (!HoloNetPlayer.Local.isServer) return;
      Debug.Log("[HOLONET] - Sending Object State Snapshot.");

      HoloNetAppModule.instance.provider.SendMessage(
        StateSyncMessage.Create(
          PackAllStates()),
        holonetPlayer, true);
    }


    private void OnStateReceived(StateSyncMessage message) {
      Debug.Log("[HOLONET] - Received Object State Snapshot.");
      roomStateToApply = message.roomState;
    }

    public void ApplyState() {
      Debug.Log("[HOLONET] - Applying object State Snapshot.");
      var objectStates = roomStateToApply.objectStates.ToDictionary(o => o.oid, o => o);

      var objectsToApplyState = new List<HoloNetObject>();
      //Create all objects
      foreach (var objectState in objectStates.Values) {
        HoloNetObject rec;
        if (HoloNetAppModule.instance.objectsManager.IsObjectRegistered(objectState.oid)) {
          rec = HoloNetAppModule.instance.objectsManager.GetObject(objectState.oid);
        } else {
          rec = HoloNetAppModule.instance.objectsManager.SpawnObjectLocal(objectState.oid, objectState.pid,
            objectState.ownerId);
        }

        objectsToApplyState.Add(rec);
      }

      //Destroy destroyed sceneObjects
      var sceneObjectsDelete = HoloNetAppModule.instance.objectsManager.GetAll()
        .Where(o => o.IsSceneObject && !objectsToApplyState.Contains(o));
      foreach (var holoNetObject in sceneObjectsDelete) {
        HoloNetAppModule.instance.objectsManager.ForceDestroyNetObject(holoNetObject);
      }

      //Apply states
      foreach (var hno in objectsToApplyState) {
        hno.SetState(objectStates[hno.oid]);
      }

      ClearRoomState();
    }

    public AsyncOp WaitAndApplySyncState(Action<AsyncOp> callback) {
      var asyncOp = new AsyncOp();
      asyncOp.callback = callback;
      AppController.instance.StartCoroutine(WaitAndApplySyncStateCor(asyncOp));
      return asyncOp;
    }

    public IEnumerator WaitAndApplySyncStateCor(AsyncOp asyncOp) {
      if (!HoloNetPlayer.Local.isServer) {
        while (!HasStateToApply()) yield return null;
        var snapshotCreationTime = 
          roomStateToApply.creationTime;
        ApplyState();
        HoloNetAppModule.instance.messenger.ClearAllMessagesBefore(snapshotCreationTime);
      }

      asyncOp.isDone = true;
      asyncOp.InvokeCallback();
    }

  }

}