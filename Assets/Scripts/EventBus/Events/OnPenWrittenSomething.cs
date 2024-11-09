using System;
using UnityEngine.Events;

namespace OnPenWrittenSomething {
    [Serializable]
    public class Event : UnityEvent<UnityEngine.Object> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when a pen is within the collision bounds for a canvas.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(UnityEngine.Object obj);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
