using HoloNetwork.Messaging;
using HoloNetwork.NetworkObjects;
using HoloNetwork.NetworkProviders;
using HoloNetwork.NetworkProviders.Photon;
using HoloNetwork.ObjectPools;
using HoloNetwork.Players;
using HoloNetwork.RoomsManagement;
using HoloNetwork.Serialization;
using HoloNetwork.Serialization.CustomTypes;
using HoloNetwork.StateSynchronization;
using HoloNetwork.TickSynchronization;
using UnityEngine;

namespace HoloNetwork {

  public class HoloNetAppModule {

    public static HoloNetAppModule instance { get; private set; }

    public PlayersManager players;
    public RoomsManager rooms;
    public INetworkProvider provider;
    public NetObjectsManager objectsManager;
    public HoloNetMessenger messenger;
    public ObjectPool objectPool;
    public StateSynchronizer stateSynchronizer;
    public TickSynchronizer tickSynchronizer;
    public ISerializer serializer;


    public void Init(string version) {
      instance = this;
      serializer = new BinaryFormatterSerializer();
      objectPool = new ObjectPool();
      provider = new PhotonNetworkProvider();
      messenger = new HoloNetMessenger();
      players = new PlayersManager();
      objectsManager = new NetObjectsManager();
      stateSynchronizer = new StateSynchronizer();
      tickSynchronizer = new TickSynchronizer();
      rooms = new RoomsManager();
      
      provider.Init(version);
      objectsManager.Init();
      stateSynchronizer.Init();
      tickSynchronizer.Init();

      messenger.Pause();
      tickSynchronizer.Pause();

      serializer.RegisterSerializationRule<Vector3>(new Vector3SerializationRule());
      serializer.RegisterSerializationRule<Quaternion>(new QuaternionSerializationRule());
      serializer.RegisterSerializationRule<HoloNetPlayer>(new HoloNetPlayerSerializationRule());  
    
    }

    public void OnUpdate() {
      tickSynchronizer.OnUpdate();
    }

    public void OnConnectedToRoom() {
      players.CleanPlayers();
      foreach (var holoNetPlayer in provider.GetPlayers()) {
        players.RegisterPlayer(holoNetPlayer);
      }

      objectsManager.RegisterSceneObjects();
    }
  }

}