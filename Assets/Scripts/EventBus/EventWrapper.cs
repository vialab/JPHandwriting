using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IEventWrapper {
    public void TryAddListener(object obj);
    public void TryRemoveListener(object obj);
}

public class EventWrapper<TEventHandler, TEvent> : IEventWrapper
where TEventHandler : IEventHandler<TEvent> {
    protected readonly TEvent UnityEvent;

    public EventWrapper(TEvent unityEvent) {
        this.UnityEvent = unityEvent;
    }

    public void TryAddListener(object obj) {
        if (obj is TEventHandler handler) {
            handler.AddListener(UnityEvent);
        }
    }    
    
    public void TryRemoveListener(object obj) {
        if (obj is TEventHandler handler) {
            handler.RemoveListener(UnityEvent);
        }
    }
}
