using System;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class VocabUI : MonoBehaviour, ILoggable {
    [FormerlySerializedAs("textUI")]
    [SerializeField] protected TextMeshProUGUI UIText;

    private Camera mainCam;

    public virtual void Show() {
        gameObject.SetActive(true);
    }

    public virtual void Hide() {
        gameObject.SetActive(false);
    }

    protected virtual void Start() {
        mainCam = Game.Instance.MainCam;
    }

    private void Update() {
        if (gameObject.activeInHierarchy) LookAtPlayer();
    }

    protected virtual void LookAtPlayer() {
        var mainCamPosition = mainCam.transform.position;

        transform.LookAt(new Vector3(mainCamPosition.x, transform.position.y, mainCamPosition.z));
        transform.forward *= -1f;
    }

    public void SetUIText(string text) {
        UIText.SetText(text);
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(transform.parent, message, level);
    }
}