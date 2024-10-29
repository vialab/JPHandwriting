using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }
    
    // TODO: add events here

    private List<IEventWrapper> events;

    private void Awake() {
        EnsureSingleton();
    }

    private void EnsureSingleton() {
        if (Instance != this && Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddListeners(object obj) {
        RemoveListeners(obj);

        foreach (var evt in events) {
            evt.TryAddListener(obj);
        }
    }

    public void RemoveListeners(object obj) {
        foreach (var evt in events) {
            evt.TryRemoveListener(obj);
        }       
    }
}
