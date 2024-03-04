using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrawStateScript : MonoBehaviour {
    [SerializeField] private GameObject _whiteboardPrefab;
    [SerializeField] private Transform _whiteboardTransform;

    private List<string> _writtenText; // stores all letters written so far

    private GameObject _currentWhiteboard;
    private void OnEnable() {
        Debug.Log("OnEnable called from DrawStateScript");
        _currentWhiteboard = Instantiate(_whiteboardPrefab, _whiteboardTransform.position, _whiteboardTransform.rotation, transform);
    }

    private void OnDisable() {
        Destroy(_currentWhiteboard);
    }
}
