
using HoloNetwork.NetworkObjects;

namespace HoloNetwork.Interfaces {

  public interface INetInitable {

    void LocalInit(HoloNetObject netObject);
    void NetInit(HoloNetObject netObject);
    void OnNetObjectDestroy();

  }

}