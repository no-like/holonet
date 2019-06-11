using System.Runtime.Serialization;

namespace HoloNetwork.Serialization {

  public interface ISerializer {

    byte[] Serialize(object obj);
    
    T Deserialize<T>(byte[] data);
    
    void RegisterSerializationRule<T>(ISerializationSurrogate serializationSurrogate);

  }

}