using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kawazu;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SenseiDesk : MonoBehaviour {
    [SerializeField] private XRSocketTagInteractor _socketInteractor;
    [SerializeField] private Hint _hintUI;

    private KawazuConverter _converter;

    private void Awake() {
        _converter = new KawazuConverter("Assets/Kawazu");
    }

    private void Start() {
        _hintUI.Hide();
    }

    private void OnDestroy() {
        _converter.Dispose(); // docs said so
    }

    public void ShowHint() {
        IXRSelectInteractable objectInSocket = _socketInteractor.GetOldestInteractableSelected();

        var kana = objectInSocket.transform.GetComponent<VocabItem>().JPName;
        
        // Kawazu's converter is async
        var uiText = Task.Run(
            async () => await GetRomaji(kana)
        );
        
        _hintUI.ShowWithHint(kana, uiText.Result);
    }

    private async Task<string> GetRomaji(string word) {
        var result = await _converter.Convert(word, To.Romaji, Mode.Spaced);
        return result;
    }
}
