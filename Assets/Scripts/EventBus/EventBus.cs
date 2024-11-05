using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }
    
    // TODO: add events here
    public OnLoggableEvent.Event OnLoggableEvent = new();
    public OnVocabItemIdleState.Event OnVocabItemIdleState = new();
    public OnVocabItemWriteState.Event OnVocabItemWriteState = new();
    public OnPenEnterCanvas.Event OnPenEnterCanvas = new();
    public OnPenWrittenSomething.Event OnPenWrittenSomething = new();
    public OnPenExitCanvas.Event OnPenExitCanvas = new();
    public OnVocabItemMenuState.Event OnVocabItemMenuState = new();
    public OnLetterPredicted.Event OnLetterPredicted = new();
    public OnLetterExported.Event OnLetterExported = new();
    public OnLetterWritten.Event OnLetterWritten = new();
    public OnVocabItemFinishGuess.Event OnVocabItemFinishGuess = new();

    private List<IEventWrapper> events;

    private void Awake() {
        EnsureSingleton();
        events = new() {
            OnLoggableEvent.Wrap(),
            OnVocabItemIdleState.Wrap(),
            OnVocabItemWriteState.Wrap(),
            OnPenEnterCanvas.Wrap(),
            OnPenWrittenSomething.Wrap(),
            OnPenExitCanvas.Wrap(),
            OnVocabItemMenuState.Wrap(),
            OnLetterPredicted.Wrap(),
            OnLetterExported.Wrap(),
            OnLetterWritten.Wrap(),
            OnVocabItemFinishGuess.Wrap()
        };
    }

    private void EnsureSingleton() {
        if (Instance != this && Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddListeners(object obj) {
        RemoveListeners(obj);

        foreach (var evt in events) {
            evt.TryAddListener(obj);
        }
    }

    public void RemoveListeners(object obj) {
        foreach (var evt in events) {
            evt.TryRemoveListener(obj);
        }       
    }
}
