using System.Collections.Generic;
using HoloNetwork.Messaging;
using UnityEngine;

namespace HoloNetwork.TickSynchronization {

  public class TickSynchronizer {

    private float _passedTime;
    private bool _isPaused = true;

    public void Init() {
      HoloNetAppModule.instance.messenger.Subscribe<TickSyncMessage>(OnTickSyncReceived);
    }

    public void OnUpdate() {
      if (_isPaused) return;
      _passedTime += Time.deltaTime;
      if (_passedTime >= HoloNetConfig.tickRate) {
        SendTickSync();
        _passedTime = 0f;
      }
    }

    public void Pause() {
      _isPaused = true;
    }

    public void Resume() {
      _passedTime = 0f;
      _isPaused = false;
    }

    private void SendTickSync() {
      var tickStatesToSend = new List<HoloNetObjectTickState>();
      foreach (var holoNetObject in HoloNetAppModule.instance.objectsManager.GetAll()) {
        if (!holoNetObject.canBeTickSynced || !holoNetObject.IsLocal ||
            !holoNetObject.gameObject.activeInHierarchy) continue;
        var tickState = holoNetObject.GatherTickState();
        if (tickState == null) continue;
//        Debug.Log("Send from: " + holoNetObject.gameObject + " | oid = " + holoNetObject.oid);
//        foreach (var componentTickState in tickState.componentStates) {
//          Debug.Log("Send message: " + componentTickState.GetType().Name + " | ComponentID = " +
//                    componentTickState.componentId);
//        }

//        Debug.Log("---------------------------------------------------------------");
        tickStatesToSend.Add(tickState);
      }

      if (tickStatesToSend.Count == 0) return;
      HoloNetAppModule.instance.messenger.SendMessage(TickSyncMessage.Create(tickStatesToSend), DestinationGroup.Others,
        false);

      foreach (var holoNetObjectTickState in tickStatesToSend) {
        foreach (var holoNetObjectComponentTickState in holoNetObjectTickState.componentStates) {
          HoloNetAppModule.instance.objectPool.Push(holoNetObjectComponentTickState);
        }

        HoloNetAppModule.instance.objectPool.Push(holoNetObjectTickState);
      }
    }

    private void OnTickSyncReceived(TickSyncMessage message) {
      if (_isPaused) return;
      foreach (var tickState in message.objectTickStates) {
        if (tickState == null) {
          Debug.Log("Received empty message");
          continue;
        }

        var obj = HoloNetAppModule.instance.objectsManager.GetObject(tickState.oid);
        if (obj == null) {
          Debug.LogError("Can't find object to apply tick state");
          continue;
        }


        Debug.Log("Receive on: " + obj.gameObject.name + " | oid = " + tickState.oid);
        foreach (var componentTickState in tickState.componentStates) {
          Debug.Log("Receive message: " + componentTickState.GetType().Name + " | ComponentID = " +
                    componentTickState.componentId);
        }

        Debug.Log("---------------------------------------------------------------");

        obj.ApplyTickState(tickState);
      }
    }

  }

}