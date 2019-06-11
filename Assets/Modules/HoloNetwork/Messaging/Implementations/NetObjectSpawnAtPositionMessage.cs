using System;
using HoloNetwork.NetworkObjects;
using UnityEngine;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class NetObjectSpawnAtPositionMessage : HoloNetGlobalMessage {

    public int prefabId;
    public HoloNetObjectId oid;
    public Vector3 position;
    public Quaternion rotation;
    public int creatorId;

    public static NetObjectSpawnAtPositionMessage Create(HoloNetObjectId oid, int prefabId, Vector3 position,
      Quaternion rotation) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetObjectSpawnAtPositionMessage>();
      obj.prefabId = prefabId;
      obj.oid = oid;
      obj.position = position;
      obj.rotation = rotation;
      obj.creatorId = HoloNetAppModule.instance.players.Local.actorId;
      return obj;
    }

  }

}