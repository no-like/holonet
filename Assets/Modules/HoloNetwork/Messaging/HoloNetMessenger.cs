using System;
using System.Collections.Generic;
using HoloNetwork.Messaging.Implementations;
using HoloNetwork.ObjectPools;
using HoloNetwork.Players;
using Misc;
using UnityEngine;

namespace HoloNetwork.Messaging {

  public class HoloNetMessenger {

    private bool _isPaused;
    private List<HoloNetMessage> _objectMessageQueue = new List<HoloNetMessage>();
    private HashSet<Type> overrideMuteMessageTypes = new HashSet<Type>();

    private TinyMessengerHub hub = new TinyMessengerHub();

    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> handler)
      where TMessage : class, ITinyMessage {
      return hub.Subscribe(handler);
    }

    //TODO remove publish
    public void Publish<TMessage>(TMessage message)
      where TMessage : class, ITinyMessage {
      hub.Publish(message);
      HoloNetAppModule.instance.objectPool.Push((IPoolObject) message);
    }

    public void Unsubscribe<TMessage>(TinyMessageSubscriptionToken token)
      where TMessage : class, ITinyMessage {
      hub.Unsubscribe<TMessage>(token);
    }

    public void SendMessage(HoloNetMessage message, DestinationGroup group, bool isReliable) {
      Debug.Log($"[HOLONET] Messaging - Sending message {message.GetType().Name}");
      message.sendTime = HoloNetAppModule.instance.provider.GetServerTime();
      if (group == DestinationGroup.All) {
        HoloNetAppModule.instance.provider.SendMessage(message, DestinationGroup.Others, isReliable);
        ApplyMessage(message);
      } else if (group == DestinationGroup.Self || (group == DestinationGroup.Server && HoloNetPlayer.Local.isServer)) {
        ApplyMessage(message);
      } else {
        HoloNetAppModule.instance.provider.SendMessage(message, group, isReliable);
      }

      HoloNetAppModule.instance.objectPool.Push(message);
    }

    public void SendMessage(HoloNetMessage message, HoloNetPlayer player, bool isReliable) {
      Debug.Log($"[HOLONET] Messaging - Sending message {message.GetType().Name}");
      message.sendTime = HoloNetAppModule.instance.provider.GetServerTime();
      if (player.isLocal) {
        ApplyMessage(message);
      } else {
        HoloNetAppModule.instance.provider.SendMessage(message, player, isReliable);
        HoloNetAppModule.instance.objectPool.Push(message);
      }
    }

    public void Pause() {
      _isPaused = true;
    }

    public void ClearAllMessagesBefore(double timeStamp) {
      _objectMessageQueue.RemoveAll(m => m.sendTime < timeStamp);
    }

    public void Resume() {
      _isPaused = false;
      foreach (var message in _objectMessageQueue) {
        ApplyMessage(message);
      }
    }

    public void RegisterMutedMessageOverride<T>() where T : HoloNetMessage {
      overrideMuteMessageTypes.Add(typeof(T));
    }

    public void OnMessageReceived(HoloNetMessage newMessage) {
      if (_isPaused && !overrideMuteMessageTypes.Contains(newMessage.GetType())) {
        Debug.Log($"[HOLONET] Messaging - Message received and put to queue: {newMessage.GetType().Name}");
        _objectMessageQueue.Add(newMessage);
      } else {
        Debug.Log($"[HOLONET] Messaging - Message received: {newMessage.GetType().Name}");
        ApplyMessage(newMessage);
      }
    }

    private void ApplyMessage(HoloNetMessage newMessage) {
//      Debug.Log($"[HOLONET] Messaging - Applying message {newMessage.GetType().Name}");
      switch (newMessage.messageType) {
        case HoloNetMessageType.GLOBAL:
          Publish(newMessage);
          break;

        case HoloNetMessageType.OBJECT:
          var objectMessage = (HoloNetObjectMessage) newMessage;
          var receiver = HoloNetAppModule.instance.objectsManager.GetObject(objectMessage.receiverId);
          if (receiver == null) break;
          receiver.PublishMessage(objectMessage);
          break;
      }
    }

  }

}