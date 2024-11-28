using System;
using UnityEditor.PackageManager;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace OnLoggableEvent {
    [Serializable]
    public class Event : UnityEvent<Object, string, LogLevel> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when the user does an action that should be loggable.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(Object obj, string text, LogLevel level = LogLevel.Info);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
