using System;
using UnityEngine.Events;

namespace OnPenEnterCanvas {
    [Serializable]
    public class Event : UnityEvent<LetterCanvas> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when a pen is within the collision bounds for a canvas.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(LetterCanvas canvas);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
