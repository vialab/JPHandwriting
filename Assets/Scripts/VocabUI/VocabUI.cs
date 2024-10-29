using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class VocabUI : MonoBehaviour {
    [FormerlySerializedAs("textUI")] 
    [SerializeField] protected TextMeshProUGUI UIText;
    
    protected abstract void Enable();
    protected abstract void Disable();

    protected void SetUIText(string text) {
        UIText.SetText(text);
    }
}