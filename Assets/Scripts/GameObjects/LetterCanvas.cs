using System.Collections;
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

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}
