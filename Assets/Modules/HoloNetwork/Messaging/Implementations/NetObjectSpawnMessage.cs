using System;
using HoloNetwork.NetworkObjects;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class NetObjectSpawnMessage : HoloNetGlobalMessage {

    public HoloNetObjectId oid;
    public int prefabId;
    public int creatorId;

    public static NetObjectSpawnMessage Create(HoloNetObjectId oid, int prefabId) {
      var obj = HoloNetAppModule.instance.objectPool.Pop<NetObjectSpawnMessage>();
      obj.prefabId = prefabId;
      obj.creatorId = HoloNetAppModule.instance.players.Local.actorId;
      obj.oid = oid;
      return obj;
    }

  }

}