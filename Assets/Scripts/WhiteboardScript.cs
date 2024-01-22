using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WhiteboardScript: MonoBehaviour {
    [SerializeField] private GameObject markerObject;

    public static event EventHandler<MarkerInteractionArgs> OnMarkerInteraction;
    public static event EventHandler OnHandwritingFinish;

    public class MarkerInteractionArgs : EventArgs {
        public bool isTouching;
    }
    
    private void Start() {
        Debug.Log("hi");
    }

    private void Update() {
        
    }

    private void OnTriggerStay(Collider other) {
        // Debug.Log(other.name);
        
        // TODO: find a better way to compare
        // if the object colliding is a marker object 
        if (other.name == markerObject.name) { 
            OnMarkerInteraction?.Invoke(this, new MarkerInteractionArgs {
                isTouching = true
            });
            Debug.Log("Ink on");
        }
    }

    private void OnTriggerExit(Collider other) {
        // TODO: find a better way to compare
        if (other.name == markerObject.name) { 
            // disable ink
            OnMarkerInteraction?.Invoke(this, new MarkerInteractionArgs {
                isTouching = false
            });
            Debug.Log("Ink off");
            
            // hit up the image exporter
            Debug.Log("Exporting image?");
            OnHandwritingFinish?.Invoke(this, EventArgs.Empty);
        }
    }
}
