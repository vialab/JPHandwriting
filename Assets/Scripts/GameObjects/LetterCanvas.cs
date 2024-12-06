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
    OnVocabItemIdleState.IHandler, OnPenWrittenSomething.IHandler, 
    OnLetterPredicted.IHandler {
    /// <summary>
    /// How long after the user's last writing event should the canvas wait until export?
    /// </summary>
    [Tooltip("How long after the user's last writing event should the canvas wait until exporting to image?")]
    [SerializeField] private float timeUntilImageExport = 1.5f;
    
    [SerializeField] private Object canvasObject;
    [FormerlySerializedAs("_canvasCube")] [SerializeField] private GameObject canvasCube;
    
    /// <summary>
    /// How long to wait for the canvas clearing cube to clear the canvas.
    /// </summary>
    [Rename("Eraser Cube Wait Time")]
    [SerializeField] protected float waitTimeSeconds = 0.15f;


    /// <summary>
    /// Whether there is something on canvas or not.
    /// </summary>
    private bool somethingOnCanvas = false;
    private Coroutine submitCoroutine;
    private VocabItem currentVocabItem = null;


    private void Hide() {
        gameObject.transform.position = Vector3.down * 10f;
    }

    private void Update() {
        // TODO: implement pushback 
        // https://www.youtube.com/watch?v=FVPnp3fTGnw
        
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
        
        EventBus.Instance.OnLetterWritten.Invoke(currentVocabItem, canvasObject);
        somethingOnCanvas = false;
    }

    private static bool ObjectIsInk(Object other) {
        return other.name.Contains("Ink");
    }
    
    /// <summary>
    /// "Clears" the canvas.
    /// Actually, spawns a big white cube for waitTimeSeconds to overwrite the contents of the canvas with pure white,
    /// and then despawns and moves the cube out of the way.
    /// </summary>
    private IEnumerator ClearCanvas() {
        var cubePosition = transform.localPosition;
        canvasCube.SetActive(true);

        LogEvent("Canvas clearing object appeared");

        // Move the cube up a bit
        canvasCube.transform.localPosition = new Vector3(
            cubePosition.x,
            cubePosition.y + 0.005f,
            cubePosition.z - 0.005f
        );

        // wait for it
        yield return new WaitForSecondsRealtime(waitTimeSeconds);

        // and move it out of the way
        // the pen ink won't show if it's just disabled
        canvasCube.transform.Translate(Vector3.down * 10f, transform.parent);
        canvasCube.SetActive(false);
        LogEvent("Canvas clearing object disappeared");
    }

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
    
    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string writtenChar, int position) {
        StartCoroutine(ClearCanvas());
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
