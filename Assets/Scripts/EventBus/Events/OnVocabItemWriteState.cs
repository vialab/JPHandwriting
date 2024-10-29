using System;
using UnityEngine.Events;

namespace OnVocabItemWriteState;

[Serializable]
public class Event : UnityEvent<VocabItem> {
    public EventWrapper<IHandler, Event> Wrap() => new(this);
}

public interface IHandler : IEventHandler<Event> {
    void OnEvent(VocabItem vocabItem);

    void IEventHandler<Event>.AddListener(Event e) => e.AddListener(OnEvent);
    void IEventHandler<Event>.RemoveListener(Event e) => e.RemoveListener(OnEvent);
}
