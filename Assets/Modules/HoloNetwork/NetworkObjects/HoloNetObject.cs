using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HoloNetwork.Attributes;
using HoloNetwork.Interfaces;
using HoloNetwork.Messaging;
using HoloNetwork.Messaging.Implementations;
using HoloNetwork.Players;
using HoloNetwork.StateSynchronization;
using HoloNetwork.TickSynchronization;
using UnityEngine;

namespace HoloNetwork.NetworkObjects {

  public class HoloNetObject : MonoBehaviour {

    private class HoloNetObjectObservable {

      public int id;
      public Component component;
      public INetInitable initable;
      public INetStateSyncable stateSyncable;
      public INetTickSyncable tickSyncable;
      public List<NetworkEventHandlerData> handlers = new List<NetworkEventHandlerData>();
      public bool isMuted;

      public bool needsTickSync => tickSyncable != null;

    }

    public HoloNetObjectId oid; //object id
    public int pid; //prefab id

    public HoloNetPlayer owner {
      get { return _owner ?? HoloNetPlayer.Server; }
      private set { _owner = value; }
    }

    private HoloNetPlayer _owner;

    public bool IsSceneObject => oid.authorId == 0;
    public bool IsLocal => owner.isLocal;

    private List<HoloNetObjectObservable> _observables = new List<HoloNetObjectObservable>();
    private int _lastObservableId;

    #region Lifecycle

    public void LocalInit(HoloNetObjectId oid, int pid, HoloNetPlayer owner) {
      this.oid = oid;
      this.pid = pid;
      this.owner = owner;
      Debug.Log($"[HOLONET] HNO - initializing new HNO {gameObject.name}");
      PopulateNetComponents();
    }

    public void LocalInitAsSceneObject() {
      Debug.Log($"[HOLONET] HNO - initializing scene HNO {gameObject.name}");
      PopulateNetComponents();
    }

    private void PopulateNetComponents() {
      foreach (var componentsInChild in GetComponentsInChildren<MonoBehaviour>()) {
        RegisterComponent(componentsInChild, false);
      }

      //In case observables will be modified during init
      var observablesCopy = new List<HoloNetObjectObservable>(_observables);
      foreach (var observable in observablesCopy) {
        if (IsLocal) {
          observable.initable?.LocalInit(this);
        } else {
          observable.initable?.NetInit(this);
        }
      }
    }

    public void OnNetDestroy() {
      foreach (var observable in _observables) {
        try {
          observable.initable?.OnNetObjectDestroy();
        } catch (Exception e) {
          Debug.LogError(e);
        }
      }
    }

    #endregion

    #region Ownership

    public void ChangeOwnership(HoloNetPlayer newOwner) {
      SendReliable(HoloNetObjectTransferOwnershipMessage.Create(newOwner.uniqueId));
    }

    [NetObjectMessageHandler(typeof(HoloNetObjectTransferOwnershipMessage))]
    public void OnOwnershipTransfered(HoloNetObjectTransferOwnershipMessage msg) {
      owner = HoloNetPlayer.FindPlayerByUniqueId(msg.newOwnerUniqueId);
    }

    #endregion

    #region Tick Sync

    public bool canBeTickSynced { get; private set; }

    public HoloNetObjectTickState GatherTickState() {
      if (!gameObject.activeInHierarchy) return null;
      var componentStates = new List<HoloNetObjectComponentTickState>();
      foreach (var holoNetObjectObservable in _observables) {
        if (!holoNetObjectObservable.needsTickSync) continue;
        if(holoNetObjectObservable.isMuted) continue;
        var tickState = holoNetObjectObservable.tickSyncable.GetTickState();
        if (tickState == null) continue;
        tickState.componentId = holoNetObjectObservable.id;
        componentStates.Add(tickState);
      }

      if (componentStates.Count == 0) return null;
      var result = HoloNetAppModule.instance.objectPool.Pop<HoloNetObjectTickState>();
      result.oid = oid;
      result.componentStates = componentStates.ToArray();
      return result;
    }

    public void ApplyTickState(HoloNetObjectTickState objectTickState) {
      foreach (var tickState in objectTickState.componentStates) {
        if (tickState == null) {
          Debug.LogError("Empty tickState for " + gameObject.name);
          continue;
        }

        //TODO add cache for observables
        var observable = _observables.FirstOrDefault(o => o.id == tickState.componentId);
        observable?.tickSyncable?.ApplyTickState(tickState);
      }
    }

    #endregion

    #region State Sync

    public HoloNetObjectState GetState() {
      var result = HoloNetAppModule.instance.objectPool.Pop<HoloNetObjectState>();
      result.oid = oid;
      result.pid = pid;
      result.ownerId = owner.actorId;
      var componentStates = new List<SerializibleNetObjectState>();
      foreach (var observable in _observables) {
        var state = observable.stateSyncable?.GetSyncState();
        if (state == null) continue;

        state.componentId = observable.id;
        componentStates.Add(state);
      }

      result.componentStates = componentStates.ToArray();
      return result;
    }

    public void SetState(HoloNetObjectState state) {
      oid = state.oid;
      pid = state.pid;
      owner = HoloNetPlayer.FindPlayer(state.ownerId);

      foreach (var componentState in state.componentStates) {
        //TODO add cache for observables
        var observable = _observables.FirstOrDefault(o => o.id == componentState.componentId);
        observable?.stateSyncable?.ApplySyncState(componentState);
      }
    }

    #endregion

    #region Messaging

    public void PublishMessage(HoloNetObjectMessage message) {
      var messageType = message.GetType();
      foreach (var observable in _observables.Where(o => !o.isMuted)) {
        foreach (var handler in observable.handlers) {
          if (handler.messageType == messageType) {
            handler.Invoke(message);
          }
        }
      }
    }

    public void SendReliable(HoloNetObjectMessage message, DestinationGroup group = DestinationGroup.All) {
      message.receiverId = oid;
      HoloNetAppModule.instance.messenger.SendMessage(message, group, true);
    }

    public void SendReliable(HoloNetObjectMessage message, HoloNetPlayer player) {
      message.receiverId = oid;
      HoloNetAppModule.instance.messenger.SendMessage(message, player, true);
    }

    public void SendUnreliable(HoloNetObjectMessage message, DestinationGroup group = DestinationGroup.All) {
      message.receiverId = oid;
      HoloNetAppModule.instance.messenger.SendMessage(message, group, false);
    }

    public void SendUnreliable(HoloNetObjectMessage message, HoloNetPlayer player) {
      message.receiverId = oid;
      HoloNetAppModule.instance.messenger.SendMessage(message, player, false);
    }

    #endregion

    #region Components Management

    public void RegisterComponents<T>(List<T> parameters, bool initImmediately = true)
      where T : Component {
      foreach (var parameter in parameters) {
        RegisterComponent(parameter, initImmediately);
      }
    }

    public T RegisterComponent<T>(T component, bool initImmediately = true)
      where T : Component {
      var observable = new HoloNetObjectObservable();
      observable.component = component;
      foreach (var methodInfo in component.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
        var attributes = methodInfo.GetCustomAttributes(false);
        var attribute = attributes.OfType<NetObjectMessageHandlerAttribute>().FirstOrDefault();
        if (attribute != null) {
          observable.handlers.Add(new NetworkEventHandlerData {
            methodInfo = methodInfo,
            messageType = attribute.messageType,
            component = component
          });
        }
      }

      observable.stateSyncable = component as INetStateSyncable;
      observable.tickSyncable = component as INetTickSyncable;
      observable.initable = component as INetInitable;

      if (observable.stateSyncable != null || observable.needsTickSync || observable.initable != null ||
          observable.handlers.Count > 0) {
        observable.id = _lastObservableId++;
        if (observable.needsTickSync) {
          canBeTickSynced = true;
        }

        if (gameObject.name == "Player(Clone)")
          Debug.Log($"[HOLONET] HNO - registering component {component}");
        _observables.Add(observable);
      }

      if (initImmediately) {
        if (IsLocal) {
          observable.initable?.LocalInit(this);
        } else {
          observable.initable?.NetInit(this);
        }
      }

      return component;
    }

    public void UnRegisterComponent(Component component) {
      var observable = _observables.FirstOrDefault(ob => ob.component == component);
      if (observable == null) return;
      _observables.Remove(observable);
      canBeTickSynced = false;
      if (observable.needsTickSync) return;
      foreach (var holoNetObjectObservable in _observables) {
        if (holoNetObjectObservable.needsTickSync) continue;
        canBeTickSynced = true;
        break;
      }
    }

    public void MuteComponent(Component component) {
      var observable = _observables.FirstOrDefault(o => o.component == component);
      if (observable == null) return;
      Debug.Log("[HOLONET] Components - MUTE: " + observable.component);
      observable.isMuted = true;
    }

    public void UnmuteComponent(Component component) {
      var observable = _observables.FirstOrDefault(o => o.component == component);
      if (observable == null) return;
      Debug.Log("[HOLONET] Components - UNMUTE: " + observable.component);
      observable.isMuted = false;
    }

    #endregion

  }

}