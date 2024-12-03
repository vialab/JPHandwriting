using System;
using UnityEngine.Events;

namespace OnLetterCompared {
    [Serializable]
    public class Event : UnityEvent<VocabItem, string, bool> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when, if tracing is enabled for the vocab item, the character written
    /// matches the one in the current position.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(VocabItem vocabItem, string character, bool isMatch);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
