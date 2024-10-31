using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class VocabUI : MonoBehaviour {
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
        // May or may not show the name of the vocabulary object
        // TODO: check this
        EventBus.Instance.OnLoggableEvent.Invoke(transform.parent, message);
    }
}