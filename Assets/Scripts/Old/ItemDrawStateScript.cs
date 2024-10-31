using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrawStateScript : MonoBehaviour {
    [SerializeField] private _BaseVocabItem _vocabObject;
    [SerializeField] private GameObject _whiteboardPrefab;
    [SerializeField] private Transform _whiteboardTransform;
    [SerializeField] private TextMeshProUGUI _writtenTextGUI;
    [SerializeField] private float _whiteboardDestroyWaitTime;
    [SerializeField] private Button _submitButton;

    // TODO: this is a Stack. refactor as such
    private List<string> _writtenText = new(); // stores all letters written so far
    private GameObject _currentWhiteboard;

    private static readonly List<string> CHIISAI_CHARACTERS = new List<string> { "や", "ゆ", "よ"};
    private static readonly List<string> CHIISAI_SMALL = new List<string> { "ゃ", "ゅ", "ょ"};

    private void OnEnable() {
        ActivityLogger.Instance.LogEvent("OnEnable called from DrawStateScript", this);
        _submitButton.onClick.AddListener(OnSubmit);
        CreateWhiteboard();
    }

    private void OnDisable() {
        DestroyWhiteboard();
        _submitButton.onClick.RemoveListener(OnSubmit);
        ResetWord(); // so people have to try again from scratch
    }
    
    private void Update() {
        // TODO: add text
        _writtenTextGUI.SetText(_writtenText is { Count: 0 } ? "" : string.Join("", _writtenText));
    }

    public void AddLetter(string character) {
        _writtenText.Add(character);
        
        // This is totally going to work guys
        StartCoroutine(DelayedCreate());
    }

    public void ToggleChiisai() {
        if (_writtenText.Count == 0) return;

        var character = _writtenText[^1];
        RemoveLetter();
        
        if (CHIISAI_CHARACTERS.Contains(character)) {
            AddLetter(CHIISAI_SMALL[CHIISAI_CHARACTERS.IndexOf(character)]);
        } else if (CHIISAI_SMALL.Contains(character)) {
            AddLetter(CHIISAI_CHARACTERS[CHIISAI_SMALL.IndexOf(character)]);
        }
    }

    public void RemoveLetter() {
        if (_writtenText.Count == 0) return;
        
        _writtenText.RemoveAt(_writtenText.Count() - 1);
    }

    private void CreateWhiteboard() {
        _currentWhiteboard = Instantiate(_whiteboardPrefab, _whiteboardTransform.position, _whiteboardTransform.rotation, transform);
        ActivityLogger.Instance.LogEvent("Whiteboard created", this);
    }

    private void ResetWord() {
        _writtenText.Clear();
    }
    
    private void DestroyWhiteboard() {
        Destroy(_currentWhiteboard);
        _currentWhiteboard = null;
        ActivityLogger.Instance.LogEvent("Whiteboard destroyed", this);
    }

    private void OnSubmit() {
        ActivityLogger.Instance.LogEvent($"Word written was {string.Join("", _writtenText)}", this);
        
        _vocabObject.ToggleLearned(string.Join("", _writtenText));
    }
    
    IEnumerator DelayedCreate() {
        DestroyWhiteboard();
        
        // This was a hacky solution to a hacky problem.
        // Camera resets after more than... 0.01f?
        yield return new WaitForSecondsRealtime(_whiteboardDestroyWaitTime);
        
        CreateWhiteboard();
    }
}
