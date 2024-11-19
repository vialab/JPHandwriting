using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : EventSubscriber, ILoggable, OnVocabItemMenuState.IHandler, OnVocabItemLearned.IHandler {
    public static Game Instance { get; private set; }

    private VocabItem currentVocabItem = null;

    public VocabItem CurrentVocabItem => currentVocabItem;
    public bool currentVocabItemTracing => currentVocabItem.TracingEnabled;
    public string currentVocabItemJPName => currentVocabItem.JPName;
    
    private List<VocabItem> _vocabItems = new();
    private List<VocabItem> _learnedVocabItems = new();

    private void Awake() {
        EnsureSingleton();
    }

    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        if (currentVocabItem != null) {
            currentVocabItem.HideUI();
            currentVocabItem = null;
        }

        if (currentVocabItem == vocabItem) return;
        
        currentVocabItem = vocabItem;
        LogEvent($"Current vocabulary item is {vocabItem} ({vocabItem.JPName})");
    }
    void OnVocabItemLearned.IHandler.OnEvent(VocabItem vocabItem) {
        _learnedVocabItems.Add(vocabItem);
        LogEvent($"{vocabItem} added to list of learned vocab items");
        
        // currentVocabItem.ToggleUI();
        // currentVocabItem = null;
    }

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
    
    private void EnsureSingleton() {
        if (Instance != this && Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
