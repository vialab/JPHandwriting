using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MarkerScript : MonoBehaviour {
    [SerializeField] private ParticleSystem markerParticleSystem;

    private bool isOnWhiteboard = false;
    
    // Start is called before the first frame update
    void Start() {
        // so there's no weird ink emission when not near a whiteboard
        markerParticleSystem.Stop();
        
        // Whiteboard will tell marker "ok you can spill your ink if the user allows"
        WhiteboardScript.OnMarkerInteraction += Whiteboard_OnMarkerInteraction;
        
        // grab interactable
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(x => StartInk());
        grabInteractable.deactivated.AddListener(x => StopInk());
    }

    // Update is called once per frame
    void Update() {
    }

    private void StartInk() {
        if (isOnWhiteboard) {
            markerParticleSystem.Play();
        } 
    }

    private void StopInk() {
        if (isOnWhiteboard) {
            markerParticleSystem.Stop();
        }
    }   
    
    private void Whiteboard_OnMarkerInteraction (object sender, WhiteboardScript.MarkerInteractionArgs markerInteraction) {
        // particle emission only starts if grip button is pressed and marker is near the board
        isOnWhiteboard = markerInteraction.isTouching;
        
        // stop the ink entirely if not on whiteboard
        if (!isOnWhiteboard) {
            markerParticleSystem.Stop();
        }
    }
}
