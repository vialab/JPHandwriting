using UnityEngine;

public abstract class LoggableObject : MonoBehaviour {
    protected static void LogEvent(Object obj, string message) {
        // May or may not show the name of the vocabulary object
        // TODO: check this
        EventBus.Instance.OnLoggableEvent.Invoke(obj, message);
    }
}