using System.Runtime.Serialization;
using UnityEngine;

namespace HoloNetwork.Serialization.CustomTypes {

  public class QuaternionSerializationRule : ISerializationSurrogate {

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
      var vec = (Quaternion) obj;
      var euler = vec.eulerAngles;
      info.AddValue("x", euler.x);
      info.AddValue("y", euler.y);
      info.AddValue("z", euler.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
      ISurrogateSelector selector) {
      return Quaternion.Euler(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
    }

  }

}