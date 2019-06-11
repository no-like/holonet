using System.Runtime.Serialization;
using UnityEngine;

namespace HoloNetwork.Serialization.CustomTypes {

  public class Vector3SerializationRule : ISerializationSurrogate {

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
      var vec = (Vector3) obj;
      info.AddValue("x", vec.x);
      info.AddValue("y", vec.y);
      info.AddValue("z", vec.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
      ISurrogateSelector selector) {
      var vec = (Vector3) obj;
      vec.x = info.GetSingle("x");
      vec.y = info.GetSingle("y");
      vec.z = info.GetSingle("z");
      return vec;
    }
  }
}