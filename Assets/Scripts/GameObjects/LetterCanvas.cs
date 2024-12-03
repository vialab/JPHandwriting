using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// The canvas the user writes a character on.
/// </summary>
public class LetterCanvas : EventSubscriber, ILoggable, OnPenWrittenSomething.IHandler {
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
    private VocabItem parentVocabItem;

    protected override void Start() {
        base.Start();
        parentVocabItem = transform.parent.GetComponent<VocabItem>();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void Update() {
        // TODO: have it always face the player, regardless of how the item is rotated
        // notes: https://gist.github.com/ezirmusitua/67bc0bc12073451b56e5ce51225b8e60
        // https://www.youtube.com/watch?v=yhB921bDLYA
        
        // TODO: implement pushback 
        // https://www.youtube.com/watch?v=FVPnp3fTGnw
        
        // TODO: moving while holding object https://discussions.unity.com/t/moving-while-holding-an-object-xr-grab-interactable/880802
    }

    private void OnTriggerEnter(Collider other) {
        if (!ObjectIsPen(other)) return;

        LogEvent("Pen collided with canvas");
            
        EventBus.Instance.OnPenEnterCanvas.Invoke(this);
        
        // if there isn't something on canvas, don't worry about it
        if (!somethingOnCanvas) return;
        
        LogEvent("No longer waiting to submit");
        StopCoroutine(submitCoroutine);
    }

    private void OnTriggerExit(Collider other) {
        if (!ObjectIsPen(other)) return;

        LogEvent("Pen left canvas");
            
        EventBus.Instance.OnPenExitCanvas.Invoke(this);

        if (somethingOnCanvas) {
            LogEvent("Waiting to submit...");
            submitCoroutine = StartCoroutine(WaitUntilSubmission());
        }
    }

    IEnumerator WaitUntilSubmission() {
        yield return new WaitForSeconds(timeUntilImageExport);
        
        EventBus.Instance.OnLetterWritten.Invoke(parentVocabItem, canvasObject);
        somethingOnCanvas = false;
    }

    private static bool ObjectIsPen(Object other) {
        return other.name.Contains("Pen");
    }
    
    void OnPenWrittenSomething.IHandler.OnEvent(Object obj) {
        somethingOnCanvas = true;
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
