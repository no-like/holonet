using System;
using System.Reflection;
using HoloNetwork.Messaging.Implementations;
using UnityEngine;

namespace HoloNetwork.Messaging {

  public class NetworkEventHandlerData {

    public Type messageType;
    public Component component;
    public MethodInfo methodInfo;

    public void Invoke(HoloNetMessage content) {
      methodInfo.Invoke(component, new[] {content});
    }

  }

}