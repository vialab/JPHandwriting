using System;
using UnityEngine.Events;

namespace OnNextTracedLetter {
    [Serializable]
    public class Event : UnityEvent<VocabItem, string> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke to show the next character,
    /// if tracing is enabled for the vocab item.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(VocabItem vocabItem, string character);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
