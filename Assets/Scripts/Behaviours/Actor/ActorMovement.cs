using System;
using HoloNetwork;
using HoloNetwork.Interfaces;
using HoloNetwork.NetworkObjects;
using HoloNetwork.StateSynchronization;
using HoloNetwork.TickSynchronization;
using UnityEngine;

namespace Behaviours.Actor {

  public class ActorMovement : MonoBehaviour,
    INetTickSyncable,
    INetInitable,
    INetStateSyncable {

    private float _speed = 5.0f;
    private HoloNetObject _net;
    
    public void LocalInit(HoloNetObject netObject) {
      _net = netObject;
    }

    public void NetInit(HoloNetObject netObject) {
      _net = netObject;
    }

    public void OnNetObjectDestroy() {
      Debug.Log("Destroying Actor");
    }
    
    private void Update() {
      if (!_net.IsLocal) return;

      if (Input.GetKey(KeyCode.W)) {
        transform.position += Vector3.forward * _speed * Time.deltaTime;
      }

      if (Input.GetKey(KeyCode.S)) {
        transform.position += Vector3.back * _speed * Time.deltaTime;
      }

      if (Input.GetKey(KeyCode.A)) {
        transform.position += Vector3.left * _speed * Time.deltaTime;
      }

      if (Input.GetKey(KeyCode.D)) {
        transform.position += Vector3.right * _speed * Time.deltaTime;
      }
    }

    [Serializable]
    public class ActorMovementTickState : HoloNetObjectComponentTickState {

      public Vector3 position;

      public static ActorMovementTickState Create(Vector3 position) {
        var obj = HoloNet.GetObjectPool().Pop<ActorMovementTickState>();
        obj.position = position;
        return obj;
      }

    }

    public HoloNetObjectComponentTickState GetTickState() {
      return ActorMovementTickState.Create(transform.position);
    }

    public void ApplyTickState(HoloNetObjectComponentTickState state) {
      var newState = (ActorMovementTickState) state;
      transform.position = newState.position;
    }
    
    [Serializable]
    public class ActorMovementSnapshot: SerializibleNetObjectState {
      public Vector3 position;
    }

    public SerializibleNetObjectState GetSyncState() {
      var snapshot = new ActorMovementSnapshot();
      snapshot.position = this.transform.position;
      return snapshot;
    }

    public void ApplySyncState(SerializibleNetObjectState state) {
      var snapshot = (ActorMovementSnapshot) state;
      this.transform.position = snapshot.position;
    }

  }

}