using HoloNetwork.NetworkObjects;
using HoloNetwork.Players;
using UnityEngine;

namespace HoloNetwork {

  public class HoloNetBehavior : MonoBehaviour {

    public bool isServer => HoloNetPlayer.Server.isLocal;
    public bool isLocal => net.IsLocal;

    private HoloNetObject _holoNetObjectCache;

    public HoloNetObject net {
      get {
        if (_holoNetObjectCache == null) _holoNetObjectCache = GetComponentInParent<HoloNetObject>();
        if (_holoNetObjectCache == null) _holoNetObjectCache = GetComponent<HoloNetObject>();
        return _holoNetObjectCache;
      }
      set { _holoNetObjectCache = value; }
    }

  }

}