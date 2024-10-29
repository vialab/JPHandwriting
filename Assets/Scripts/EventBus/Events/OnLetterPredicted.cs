using System;
using UnityEngine.Events;

namespace OnLetterPredicted;

[Serializable]
public class Event : UnityEvent<VocabItem, string> {
    public EventWrapper<IHandler, Event> Wrap() => new(this);
}

/// <summary>
/// The event to invoke when the API has sent back a guess for the letter the user wrote.
/// </summary>
public interface IHandler : IEventHandler<Event> {
    void OnEvent(VocabItem vocabItem, string character);

    void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
    void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
}
