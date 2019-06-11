using System;
using HoloNetwork.NetworkObjects;

namespace HoloNetwork.Messaging.Implementations {

  [Serializable]
  public class HoloNetObjectIdMessage<T, TO> : HoloNetObjectMessage
    where T : HoloNetObjectIdMessage<T, TO>, new() where TO : HoloNetBehavior {

    public HoloNetObjectId id;

    public TO actualObject => HoloNet.GetObject<TO>(id);


    public static HoloNetObjectIdMessage<T, TO> Create(HoloNetObjectId objectId) {
      var obj = HoloNet.GetObjectPool().Pop<T>();
      obj.id = objectId;
      return obj;
    }

    public static HoloNetObjectIdMessage<T, TO> Create(TO actualObject) {
      var obj = HoloNet.GetObjectPool().Pop<T>();
      obj.id = actualObject.net.oid;
      return obj;
    }

  }

}