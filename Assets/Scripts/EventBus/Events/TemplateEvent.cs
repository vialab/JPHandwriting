using System;
using UnityEngine;
using UnityEngine.Events;

namespace TemplateEvent {
    [Serializable]
    public class Event : UnityEvent<GameObject> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    public interface IHandler : IEventHandler<Event> {
        void OnEvent(GameObject obj);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
