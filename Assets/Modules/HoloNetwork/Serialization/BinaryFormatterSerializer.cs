using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace HoloNetwork.Serialization {

  public class BinaryFormatterSerializer : ISerializer {

    private readonly BinaryFormatter _binaryFormatter;
    private readonly SurrogateSelector _surrogateSelector;
    private readonly MemoryStream _stream;

    public BinaryFormatterSerializer() {
      _binaryFormatter = new BinaryFormatter();
      _surrogateSelector = new SurrogateSelector();
      _binaryFormatter.SurrogateSelector = _surrogateSelector;
      _stream = new MemoryStream();
    }

    public byte[] Serialize(object obj) {
      byte[] serializedData = new byte[0];
      try {
        _stream.Position = 0;
        _binaryFormatter.Serialize(_stream, obj);
        serializedData = _stream.ToArray();
        _stream.SetLength(0);
      } catch (Exception e) {
        Debug.LogError($"[HOLONET] Serializer - Error while serializing {obj}. Original error {e.StackTrace}");
      }

      return serializedData;
    }

    public T Deserialize<T>(byte[] data) {
      _stream.Write(data, 0, data.Length);
      _stream.Position = 0;
      var result = _binaryFormatter.Deserialize(_stream);
      _stream.SetLength(0);
      return (T) result;
    }

    public void RegisterSerializationRule<T>(ISerializationSurrogate serializationSurrogate) {
      _surrogateSelector.AddSurrogate(typeof(T), new StreamingContext(StreamingContextStates.All),
        serializationSurrogate);
    }

  }

}