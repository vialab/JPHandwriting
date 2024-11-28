using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class VocabUI : MonoBehaviour, ILoggable {
    [FormerlySerializedAs("textUI")] 
    [SerializeField] protected TextMeshProUGUI UIText;
    
    public virtual void Show() {
        gameObject.SetActive(true);
    }

    public virtual void Hide() {
        gameObject.SetActive(false);
    }

    public void SetUIText(string text) {
        UIText.SetText(text);
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(transform.parent, message, level);
    }
}