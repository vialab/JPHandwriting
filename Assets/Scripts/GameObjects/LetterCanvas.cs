using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

// TODO: handle timer for pen contact and letter prediction trigger

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

        if (somethingOnCanvas) {
            StopCoroutine(submitCoroutine);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!ObjectIsPen(other)) return;

        LogEvent("Pen left canvas");
            
        EventBus.Instance.OnPenExitCanvas.Invoke(this);

        if (somethingOnCanvas) {
            submitCoroutine = StartCoroutine(WaitUntilSubmission());
        }
    }

    IEnumerator WaitUntilSubmission() {
        yield return new WaitForSeconds(timeUntilImageExport);
        
        EventBus.Instance.OnLetterWritten.Invoke(transform.parent, canvasObject);
        somethingOnCanvas = false;
    }

    private bool ObjectIsPen(Object other) {
        return other.name.Equals("Pen");
    }
    
    void OnPenWrittenSomething.IHandler.OnEvent(Object obj) {
        somethingOnCanvas = true;
    }

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}
