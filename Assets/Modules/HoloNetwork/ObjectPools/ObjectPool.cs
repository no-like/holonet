using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoloNetwork.ObjectPools {

  public class ObjectPool {

    private readonly Dictionary<Type, List<IPoolObject>> _cache = new Dictionary<Type, List<IPoolObject>>();

    public void Push(IPoolObject val) {
      if (val == null) return;
      val.InUse = false;
    }

    public virtual T Pop<T>() where T : IPoolObject {
      IPoolObject result = null;
      var t = typeof(T);
      if (_cache.ContainsKey(t)) {
        result = _cache[t].FirstOrDefault(item => !item.InUse);
      }

      if (result == null) {
        result = (IPoolObject) Activator.CreateInstance(t);
        AddToCache(result);
      }

      result.InUse = true;
      return (T) result;
    }

    private void AddToCache(IPoolObject val) {
      if (!_cache.ContainsKey(val.GetType())) {
        _cache[val.GetType()] = new List<IPoolObject>();
      }

      _cache[val.GetType()].Add(val);
    }

  }

}