using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SenseiDesk : MonoBehaviour {
    [SerializeField] private XRSocketTagInteractor _socketInteractor;
    [SerializeField] private Hint _hintUI;

    private void Start() {
    }

    public void ShowHint() {
        IXRSelectInteractable objectInSocket = _socketInteractor.GetOldestInteractableSelected();
        
        _hintUI.SetUIText(objectInSocket.transform.name);
        _hintUI.Show();
    }
}
