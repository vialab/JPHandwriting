using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class VocabUI : LoggableObject {
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

    protected void LogEvent(string message) {
        // Surely this works
        LogEvent(transform.parent, message);
    }
}