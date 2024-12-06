using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// The canvas the user writes a character on.
/// </summary>
public class LetterCanvas : EventSubscriber, ILoggable, 
    OnVocabItemWriteState.IHandler, OnVocabItemMenuState.IHandler,
    OnVocabItemIdleState.IHandler, OnPenWrittenSomething.IHandler {
    /// <summary>
    /// How long after the user's last writing event should the canvas wait until export?
    /// </summary>
    [Tooltip("How long after the user's last writing event should the canvas wait until exporting to image?")]
    [SerializeField] private float timeUntilImageExport = 1.5f;
    
    [SerializeField] private Object canvasObject;
    
    /// <summary>
    /// Whether there is something on canvas or not.
    /// </summary>
    private bool somethingOnCanvas = false;
    private Coroutine submitCoroutine;
    private VocabItem currentVocabItem = null;


    private void Hide() {
        // TODO: this but better
        transform.parent.transform.localPosition = Vector3.down * 10f;
    }

    private void OnTriggerEnter(Collider other) {
        if (!ObjectIsInk(other)) return;

        LogEvent("Pen collided with canvas");
            
        EventBus.Instance.OnPenEnterCanvas.Invoke(this);
        
        // if there isn't something on canvas, don't worry about it
        if (!somethingOnCanvas) return;
        
        LogEvent("No longer waiting to submit");
        StopCoroutine(submitCoroutine);
    }

    private void OnTriggerExit(Collider other) {
        if (!ObjectIsInk(other)) return;

        LogEvent("Pen left canvas");
            
        EventBus.Instance.OnPenExitCanvas.Invoke(this);

        if (!somethingOnCanvas) return;
        LogEvent("Waiting to submit...");
        submitCoroutine = StartCoroutine(WaitUntilSubmission());
    }

    private IEnumerator WaitUntilSubmission() {
        yield return new WaitForSeconds(timeUntilImageExport);

        currentVocabItem = Game.Instance.CurrentVocabItem;
        
        EventBus.Instance.OnLetterWritten.Invoke(currentVocabItem, canvasObject);
        somethingOnCanvas = false;
    }

    private static bool ObjectIsInk(Object other) {
        return other.name.Contains("Ink");
    }
    
    

    // TODO: figure out why this isn't being called
    void OnVocabItemWriteState.IHandler.OnEvent(VocabItem vocabItem) {
        currentVocabItem = vocabItem;
        LogEvent($"Write state activated from {vocabItem}", LogLevel.Debug);
    }

    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        if (!gameObject.activeInHierarchy) return;
        Hide();
    }

    void OnVocabItemIdleState.IHandler.OnEvent(VocabItem vocabItem) {
        if (!gameObject.activeInHierarchy) return;
        Hide();
    }
    
    void OnPenWrittenSomething.IHandler.OnEvent(Object obj) {
        somethingOnCanvas = true;
    }
    
    

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
