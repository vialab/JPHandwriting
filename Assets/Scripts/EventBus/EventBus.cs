using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }
    
    // TODO: add events here
    // Logging stuff
    public OnLoggableEvent.Event OnLoggableEvent = new();
    
    // Vocab item stuff
    public OnVocabItemIdleState.Event OnVocabItemIdleState = new();
    public OnVocabItemMenuState.Event OnVocabItemMenuState = new();
    public OnVocabItemWriteState.Event OnVocabItemWriteState = new();
    public OnVocabItemFinishGuess.Event OnVocabItemFinishGuess = new();
    public OnVocabItemLearned.Event OnVocabItemLearned = new();
    
    // Pen stuff
    public OnPenEnterCanvas.Event OnPenEnterCanvas = new();
    public OnPenWrittenSomething.Event OnPenWrittenSomething = new();
    public OnPenExitCanvas.Event OnPenExitCanvas = new();
    
    // Vocab item *character* stuff
    public OnLetterCompared.Event OnLetterCompared = new();
    public OnLetterPredicted.Event OnLetterPredicted = new();
    public OnLetterExported.Event OnLetterExported = new();
    public OnLetterWritten.Event OnLetterWritten = new();

    private List<IEventWrapper> events;

    private void Awake() {
        EnsureSingleton();
        events = new() {
            OnLoggableEvent.Wrap(),
            
            OnVocabItemIdleState.Wrap(),
            OnVocabItemMenuState.Wrap(),
            OnVocabItemWriteState.Wrap(),
            OnVocabItemFinishGuess.Wrap(),
            OnVocabItemLearned.Wrap(),
            
            OnPenEnterCanvas.Wrap(),
            OnPenWrittenSomething.Wrap(),
            OnPenExitCanvas.Wrap(),

            OnLetterCompared.Wrap(),
            OnLetterPredicted.Wrap(),
            OnLetterExported.Wrap(),
            OnLetterWritten.Wrap(),
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
