using System;
using UnityEngine.Events;

namespace OnVocabItemFinishGuess {
    [Serializable]
    public class Event : UnityEvent<VocabItem, string> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    public interface IHandler : IEventHandler<Event> {
        void OnEvent(VocabItem vocabItem, string guess);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
