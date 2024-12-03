using System;
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

    private void Update() {
        // TODO: have it always face the player, regardless of how the item is rotated
        // notes: https://gist.github.com/ezirmusitua/67bc0bc12073451b56e5ce51225b8e60
        // https://www.youtube.com/watch?v=yhB921bDLYA
    }

    public void SetUIText(string text) {
        UIText.SetText(text);
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(transform.parent, message, level);
    }
}