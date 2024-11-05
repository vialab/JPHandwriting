using System;
using UnityEngine;
using Object = UnityEngine.Object;

// TODO: handle timer for pen contact and letter prediction trigger

/// <summary>
/// The canvas the user writes a character on.
/// </summary>
public class LetterCanvas : MonoBehaviour, ILoggable {
    /// <summary>
    /// How long after the user's last writing event should the canvas wait until export?
    /// </summary>
    [Tooltip("How long after the user's last writing event should the canvas wait until exporting to image?")]
    [SerializeField] private float timeUntilImageExport = 1.5f;

    /// <summary>
    /// Whether the pen is touching the canvas or not.
    /// </summary>
    private bool isPenTouchingCanvas = false;

    private bool ObjectIsPen(Object other) {
        return other.name.Equals("Pen");
    }

    private void OnTriggerEnter(Collider other) {
        if (!ObjectIsPen(other)) return;

        LogEvent("Pen collided with canvas");
            
        EventBus.Instance.OnPenEnterCanvas.Invoke(this);
        isPenTouchingCanvas = true;
    }

    private void OnTriggerExit(Collider other) {
        if (!ObjectIsPen(other)) return;

        LogEvent("Pen left canvas");
            
        EventBus.Instance.OnPenExitCanvas.Invoke(this);
        isPenTouchingCanvas = false;
    }

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}
