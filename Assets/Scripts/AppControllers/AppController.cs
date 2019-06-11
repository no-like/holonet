using HoloNetwork;
using UnityEngine;

namespace AppControllers {

  public class AppController : MonoBehaviour {

    public static AppController instance;

    public HoloNetAppModule holonet;

    public void Awake() {
      instance = this;
    }

    public void Start() {
      holonet = new HoloNetAppModule();
      holonet.Init(HoloNetConfig.gameVersion);
    }

  }

}