using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeacherDeskScript : MonoBehaviour {
    [SerializeField] private XRSocketTagInteractor _socketInteractor;
    [SerializeField] private GameObject _uiObject;
    [SerializeField] private TextMeshProUGUI _hintText;

    private void Start() {
        _uiObject.SetActive(false);
    }

    public void DisplayHint() {
        IXRSelectInteractable objectInSocket = _socketInteractor.GetOldestInteractableSelected();
        Debug.Log($"This is a(n) {objectInSocket.transform.name} in the {transform.name}");
        _hintText.SetText(objectInSocket.transform.GetComponent<_BaseVocabItem>().japaneseRomaji);
        
        _uiObject.SetActive(true);
    }

    public void HideHint() {
        _hintText.SetText("");
        _uiObject.SetActive(false);
    }
}
