using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class VocabItemBounds : MonoBehaviour, ILoggable {
    private VocabItem _vocabItem;

    private void Awake() {
        _vocabItem = transform.parent.GetComponent<VocabItem>();
    }

    private void OnTriggerExit(Collider other) {
        // will recursively look upwards until it finds a VocabItem
        var otherParentVocabItem = other.transform.GetComponentInParent<VocabItem>();
        
        LogEvent($"{other.name} left bounds, parent is {otherParentVocabItem.Type}");
        
        if (otherParentVocabItem != _vocabItem) return;
        
        LogEvent($"Disabling item", LogLevel.Debug);
        _vocabItem.ShowIdleState();
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(_vocabItem, message, level);
    }
}
