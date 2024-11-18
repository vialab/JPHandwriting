using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : EventSubscriber, ILoggable, OnVocabItemMenuState.IHandler, OnVocabItemLearned.IHandler {
    private VocabItem currentVocabItem = null;
    private List<VocabItem> _vocabItems = new();
    private List<VocabItem> _learnedVocabItems = new();
    
    protected override void Start() {
        base.Start();
    }

    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        if (currentVocabItem != null) 
            currentVocabItem.ToggleUI();
        
        currentVocabItem = vocabItem;
        LogEvent($"Current vocabulary item is {vocabItem}");
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

}
