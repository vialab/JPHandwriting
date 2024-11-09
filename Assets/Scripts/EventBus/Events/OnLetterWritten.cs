using System;
using UnityEngine;
using UnityEngine.Events;

namespace OnLetterWritten {
    [Serializable]
    public class Event : UnityEvent<UnityEngine.Object, UnityEngine.Object> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when the user is finished writing a letter.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(UnityEngine.Object obj, UnityEngine.Object objWithRenderTexture);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}