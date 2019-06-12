using System;
using System.Collections;
using System.Linq;
using HoloNetwork;
using HoloNetwork.Messaging.Implementations.ProviderMessages;
using HoloNetwork.Players;
using HoloNetwork.RoomsManagement;
using UnityEngine;

namespace AppControllers {

  public class AppController : MonoBehaviour {

    public static AppController instance;

    public HoloNetAppModule holonet;

    public void Awake() {
      instance = this;
    }
    
    public IEnumerator CreateOrJoinRoom(string roomName) {
      var roomsList = holonet.provider.GetRoomList("public");
      while (!roomsList.isDone) yield return null;
      var targetRoom = roomsList.rooms.FirstOrDefault(item => item.name == HoloNetConfig.testRoomName);
      Action<HoloNetwork.Helper.AsyncOperations.AsyncOp> spawnActor = delegate {
        HoloNet.WaitAndApplySyncState();
        holonet.tickSynchronizer.Resume();
        holonet.messenger.Resume();
      };
      if (targetRoom == null) {
        holonet.provider.CreateAndJoinRoom(new RoomSettings()
            {name = HoloNetConfig.testRoomName, roomType = "public", isOpen = true, isVisible = true, maxPlayers = 6},
          spawnActor);
      } else {
        holonet.provider.JoinRoom(targetRoom.id, spawnActor);
      }
    }

    public void Start() {
      holonet = new HoloNetAppModule();
      holonet.Init(HoloNetConfig.gameVersion);
      
      holonet.provider.Connect(item => { StartCoroutine(CreateOrJoinRoom(HoloNetConfig.testRoomName)); });

      HoloNetAppModule.instance.messenger.Subscribe<NetPlayerDisconnectedMessage>(OnNetPlayerDisconnected);
    }

    public void OnNetPlayerDisconnected(NetPlayerDisconnectedMessage msg) {
      Debug.Log("Player Disconnected " + msg.player);
      
      var objs = holonet.objectsManager.FindAllObjectsAuthoredBy(msg.player);
      Debug.Log($"FOUND OBJECTS BY AUTHOR, COUNT {objs.Count()}. Destroying...");
      foreach (var holoNetObject in objs.ToArray()) {
        holonet.objectsManager.DestroyNetObject(holoNetObject);
      }
      
      holonet.objectsManager.CleanUpDestroyedObjects();
    }

    private void Update() {
      //start tick sync in particular
      holonet.OnUpdate();

      if (Input.GetKeyDown(KeyCode.K)) {
        holonet.objectsManager.Spawn(Resources.Load<GameObject>("NetObjects/Actor"));
      }

      if (Input.GetKeyDown(KeyCode.L)) {
        var objs = holonet.objectsManager.FindAllObjectsAuthoredBy(HoloNetPlayer.Local);
        foreach (var holoNetObject in objs.ToArray()) {
          holonet.objectsManager.DestroyNetObject(holoNetObject);
        }
      }
    }

  }

}