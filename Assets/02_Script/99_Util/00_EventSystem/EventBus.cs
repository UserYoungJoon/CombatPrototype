using UnityEngine;
using System;
using System.Collections.Generic;

public class EventBus : MonoBehaviour
{
    private Dictionary<Type, List<Delegate>> _subscriptions = new Dictionary<Type, List<Delegate>>();

    public Action<T> ConnectEvent<T>(Action<T> handler) where T : struct, IGameEvent
    {
        if (!_subscriptions.ContainsKey(typeof(T)))
            _subscriptions[typeof(T)] = new List<Delegate>();

        // 중복 방지: 이미 등록된 핸들러는 다시 등록하지 않음
        if (!_subscriptions[typeof(T)].Contains(handler))
            _subscriptions[typeof(T)].Add(handler);

        return handler;
    }

    public void DisconnectEvent<T>(Action<T> handler) where T : struct, IGameEvent
    {
        if (_subscriptions.TryGetValue(typeof(T), out var subs))
            subs.Remove(handler);
    }

    public void SendEvent<T>(T eventData) where T : struct, IGameEvent
    {
        if (!_subscriptions.TryGetValue(typeof(T), out var handlers))
            return;

        // 역순 순회로 순회 중 Remove 안전하게 처리
        for (int i = handlers.Count - 1; i >= 0; i--)
        {
            ((Action<T>)handlers[i]).Invoke(eventData);
        }
    }

    private void OnDestroy()
    {
        _subscriptions.Clear();
    }
}
