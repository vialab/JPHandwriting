using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MarkerScript : MonoBehaviour {
    [SerializeField] private ParticleSystem markerParticleSystem;

    private bool _isOnWhiteboard = false;

    private XRGrabInteractable _grabInteractable;
    
    private void Awake() {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    // Start is called before the first frame update
    void Start() {
        // so there's no weird ink emission when not near a whiteboard
        markerParticleSystem.Stop();
        
        // Whiteboard will tell marker "ok you can make ink if the user allows"
        WhiteboardScript.OnMarkerInteraction += Whiteboard_OnMarkerInteraction;
        
        // grab interactable listeners
        _grabInteractable.activated.AddListener(x => StartInk());
        _grabInteractable.deactivated.AddListener(x => StopInk());
    }

    // Update is called once per frame
    void Update() {
    }

    private void StartInk() {
        if (_isOnWhiteboard) {
            ActivityLogger.Instance.LogEvent("Ink started", this);
            markerParticleSystem.Play();
            
        } 
    }

    private void StopInk() {
        ActivityLogger.Instance.LogEvent("Ink stopped", this);
        markerParticleSystem.Stop();
    }   
    
    private void Whiteboard_OnMarkerInteraction (object sender, WhiteboardScript.MarkerInteractionArgs markerInteraction) {
        // particle emission only starts if grip button is pressed and marker is near the board
        _isOnWhiteboard = markerInteraction.isTouching;
        
        // stop the ink entirely if not on whiteboard
        if (!_isOnWhiteboard) {
            StopInk();
        }
    }
}
