using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ItemDrawStateScript : MonoBehaviour {
    [SerializeField] private GameObject _whiteboardPrefab;
    [SerializeField] private Transform _whiteboardTransform;
    [SerializeField] private TextMeshProUGUI _writtenTextGUI;

    private List<string> _writtenText; // stores all letters written so far

    private GameObject _currentWhiteboard;
    private void OnEnable() {
        Debug.Log("OnEnable called from DrawStateScript");
        _currentWhiteboard = Instantiate(_whiteboardPrefab, _whiteboardTransform.position, _whiteboardTransform.rotation, transform);
    }

    private void OnDisable() {
        Destroy(_currentWhiteboard);
    }
    
    // This is great code
    private void Update() {
        // TODO: add text
        _writtenTextGUI.SetText(string.Join("", _writtenText));
    }

    public void AddLetter(string character) {
        _writtenText.Add(character);
    }
}
