using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloNetwork.Messaging;
using HoloNetwork.Messaging.Implementations;
using HoloNetwork.Messaging.Implementations.ProviderMessages;
using HoloNetwork.NetworkObjects.Exceptions;
using HoloNetwork.Players;
using UnityEngine;

namespace HoloNetwork.NetworkObjects {

  public class NetObjectsManager {

    private GameObject[] _netPrefabs;
    private Dictionary<HoloNetObjectId, HoloNetObject> _netObjects;
    private uint _lastAllocatedId;
    public int netObjectsCount => _netObjects.Count;

    public void Init() {
      _netObjects = new Dictionary<HoloNetObjectId, HoloNetObject>();
      HoloNetAppModule.instance.messenger.Subscribe<NetObjectSpawnMessage>(OnSpawnObject);
      HoloNetAppModule.instance.messenger.Subscribe<NetObjectSpawnAtPositionMessage>(OnSpawnNetObjectAtPosition);
      HoloNetAppModule.instance.messenger.Subscribe<NetObjectDestroyMessage>(OnObjectDestroy);
      HoloNetAppModule.instance.messenger.Subscribe<NetDisconnectMessage>(OnNetDisconnect);

      _netPrefabs = Resources
        .LoadAll<HoloNetObject>(HoloNetConfig.netObjectsPath)
        .Select(hno => hno.gameObject)
        .ToArray();
    }

    #region Spawn

    public HoloNetObject Spawn(MonoBehaviour comp, Vector3 position, Quaternion rotation) {
      return Spawn(comp.gameObject, position, rotation);
    }

    public HoloNetObject Spawn(GameObject go, Vector3 position, Quaternion rotation) {
      if (go == null) throw new HoloNetObjectSpawnException("[HOLONET] - Prefab to spawn is null");
      var prefabIndex = GetPrefabIndex(go);
      var oid = AllocateObjectId();
      HoloNetAppModule.instance.messenger.SendMessage(
        NetObjectSpawnAtPositionMessage.Create(oid, prefabIndex, position, rotation), DestinationGroup.Others,
        true);
      return SpawnObjectLocal(oid, prefabIndex, HoloNetAppModule.instance.players.Local.actorId, position,
        rotation);
    }

    public HoloNetObject Spawn(MonoBehaviour comp) {
      return Spawn(comp.gameObject);
    }

    public HoloNetObject Spawn(GameObject go) {
      if (go == null) throw new HoloNetObjectSpawnException("[HOLONET] - Prefab to spawn is null");
      var oid = AllocateObjectId();
      var prefabIndex = GetPrefabIndex(go);
      HoloNetAppModule.instance.messenger.SendMessage(NetObjectSpawnMessage.Create(oid, prefabIndex),
        DestinationGroup.Others, true);
      return SpawnObjectLocal(oid, prefabIndex, HoloNetAppModule.instance.players.Local.actorId);
    }

    private void OnSpawnObject(NetObjectSpawnMessage msg) {
      SpawnObjectLocal(msg.oid, msg.prefabId, msg.creatorId);
    }

    private void OnSpawnNetObjectAtPosition(NetObjectSpawnAtPositionMessage msg) {
      SpawnObjectLocal(msg.oid, msg.prefabId, msg.creatorId, msg.position, msg.rotation);
    }

    public HoloNetObject SpawnObjectLocal(HoloNetObjectId oid, int prefabId, int ownerId,
      Vector3 position = new Vector3(),
      Quaternion rotation = new Quaternion()) {
      var prefab = GetPrefabById(prefabId);
      var result = Object.Instantiate(prefab, position, rotation).GetComponent<HoloNetObject>();
      result.transform.position = position;
      result.transform.rotation = rotation;

      RegisterObject(oid, result);
      result.LocalInit(oid, prefabId, HoloNetPlayer.FindPlayer(ownerId));

      return result;
    }

    #endregion

    #region Destruction

    public void DestroyNetObject(HoloNetObject obj) {
      HoloNetAppModule.instance.messenger
        .SendMessage(NetObjectDestroyMessage.Create(obj.oid), DestinationGroup.All, true);
    }

    public void ForceDestroyNetObject(HoloNetObject holoNetObject) {
      DestroyObject(holoNetObject);
    }

    private void OnObjectDestroy(NetObjectDestroyMessage msg) {
      var netObj = GetObject(msg.oid);
      DestroyObject(netObj);
    }

    private void DestroyObject(HoloNetObject obj) {
      obj.OnNetDestroy();
      _netObjects.Remove(obj.oid);
      Object.Destroy(obj.gameObject, 0f);
    }

    #endregion

    #region Getting

    public HoloNetObject GetObject(HoloNetObjectId oid) {
      HoloNetObject result;
      _netObjects.TryGetValue(oid, out result);
      return result;
    }


    public IEnumerable<HoloNetObject> FindAllObjectsAuthoredBy(HoloNetPlayer author) {
      return _netObjects.Values.Where(hno => hno.oid.authorId == author.uniqueId);
    }

    private List<HoloNetObject> GetAllObjectsOwnedBy(int ownerActorId) {
      var list = new List<HoloNetObject>();
      foreach (var item in _netObjects) {
        if (item.Value.owner.actorId == ownerActorId) {
          list.Add(item.Value);
        }
      }

      return list;
    }

    public IEnumerable<HoloNetObject> GetAll() {
      return _netObjects.Values;
    }

    #endregion

    private void TransferObjectsOwnership(int oldOwnerActorId, int newOwnerActorId) {
      var netObjects = GetAllObjectsOwnedBy(oldOwnerActorId);
      var newOwner = HoloNetAppModule.instance.players.GetPlayerByActorId(newOwnerActorId);
      netObjects.ForEach(item => item.ChangeOwnership(newOwner));
    }

    //TODO Optimize
    private HoloNetObjectId AllocateObjectId() {
      var authoredObjects = FindAllObjectsAuthoredBy(HoloNetPlayer.Local).ToList();
      if (authoredObjects.Count == 0) {
        _lastAllocatedId = 0;
      } else {
        _lastAllocatedId = authoredObjects.Max(hno => hno.oid.objectId);
      }

      return new HoloNetObjectId(HoloNetAppModule.instance.players.Local.uniqueId, _lastAllocatedId + 1);
    }

    private void CleanUpAllObjectsLocally() {
      foreach (var item in _netObjects) {
        GameObject.Destroy(item.Value.gameObject);
      }

      _netObjects.Clear();
    }

    #region Registering

    private void RegisterObject(HoloNetObjectId oid, HoloNetObject obj) {
      _netObjects.Add(oid, obj);
    }


    public void RegisterSceneObjects() {
      foreach (var netObject in GameObject.FindObjectsOfType<HoloNetObject>()) {
        if (netObject.IsSceneObject && !IsObjectRegistered(netObject.oid)) {
          RegisterObject(netObject.oid, netObject);
          netObject.LocalInitAsSceneObject();
        } else {
          if (netObject.IsSceneObject)
            Debug.LogError(
              $"[HOLONET] - Scene Object {netObject.gameObject.name} with id {netObject.oid} already registered");
        }
      }
    }

    public IEnumerator RegisterSceneObjectsContinious() {
      foreach (var netObject in GameObject.FindObjectsOfType<HoloNetObject>()) {
        if (netObject.IsSceneObject && !IsObjectRegistered(netObject.oid)) {
          RegisterObject(netObject.oid, netObject);
          netObject.LocalInitAsSceneObject();
          yield return null;
        } else {
          if (netObject.IsSceneObject)
            Debug.LogError(
              $"[HOLONET] - Scene Object {netObject.gameObject.name} with id {netObject.oid} already registered");
        }
      }
    }


    public bool IsObjectRegistered(HoloNetObjectId oid) {
      return _netObjects.ContainsKey(oid);
    }

    #endregion

    private void OnNetDisconnect(NetDisconnectMessage msg) {
      CleanUpAllObjectsLocally();
    }


    #region Prefabs

    //TODO add null check
    private GameObject GetPrefabById(int id) {
      return _netPrefabs[id];
    }

    private int GetPrefabIndex(GameObject go) {
      for (var i = 0; i < _netPrefabs.Length; i++) {
        if (_netPrefabs[i] == go) return i;
      }

      throw new HoloNetObjectSpawnException(
        $"[HOLONET] - Prefab index for ({go.name}) not found. Check if it has HoloNetObject.");
    }

    #endregion

    public void CleanUpDestroyedObjects() {
      var idsToCleanup = new List<HoloNetObjectId>();
      foreach (var kvp in _netObjects) {
        if (kvp.Value == null) idsToCleanup.Add(kvp.Key);
      }

      foreach (var holoNetObjectId in idsToCleanup) {
        _netObjects.Remove(holoNetObjectId);
      }
    }

  }

}