using System;
using UnityEngine.Events;

namespace OnVocabItemMenuState {
    [Serializable]
    public class Event : UnityEvent<VocabItem> {
        public EventWrapper<IHandler, Event> Wrap() => new(this);
    }

    /// <summary>
    /// The event to invoke when a vocab item enters Menu state.
    /// </summary>
    public interface IHandler : IEventHandler<Event> {
        void OnEvent(VocabItem vocabItem);

        void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
        void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
    }
}
