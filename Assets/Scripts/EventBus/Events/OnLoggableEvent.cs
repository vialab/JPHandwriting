using System;
using UnityEngine.Events;

namespace OnLoggableEvent {
    [Serializable]
    public class Event : UnityEvent<object, string> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when the user does an action that should be loggable.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(object obj, string text);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
