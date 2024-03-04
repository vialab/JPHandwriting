using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WhiteboardScript: MonoBehaviour {
    [SerializeField] private GameObject markerObject;
    [SerializeField] private float timeUntilImageCreate = 1.5f;
    
    public static event EventHandler<MarkerInteractionArgs> OnMarkerInteraction;
    public static event EventHandler OnHandwritingFinish;

    public class MarkerInteractionArgs : EventArgs {
        public bool isTouching;
    }

    private float timeOffCanvas = 0f;
    private bool canvasIsEmpty = true;
    private bool isTouching = false;
    private bool isFinishing = false;
    private XRGrabInteractable _xrGrabInteractable;

    private void Awake() {
    }

    private void Start() {
        
    }

    private void Update() {
        if (isFinishing) {
            timeOffCanvas += Time.deltaTime;

            if (timeOffCanvas > timeUntilImageCreate) {
                Debug.Log("It's been a few seconds");
                timeOffCanvas = 0f;

                isFinishing = false;
                
                // hit up the image exporter
                Debug.Log("Exporting image?");
                OnHandwritingFinish?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    
    private void OnTriggerStay(Collider other) {
        // Debug.Log(other.name);
        
        // if the object colliding is a marker object 
        if (other.name == markerObject.name) {
            isTouching = true;
            
            // sanity check
            isFinishing = false;
            timeOffCanvas = 0f;
            
            // enable ink
            OnMarkerInteraction?.Invoke(this, new MarkerInteractionArgs {
                isTouching = isTouching
            });
            Debug.Log("Ink on");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name == markerObject.name) {
            isTouching = false;
            isFinishing = true;
               
            // disable ink
            OnMarkerInteraction?.Invoke(this, new MarkerInteractionArgs {
                isTouching = isTouching
            });
            Debug.Log("Ink off");

        }
    }
}
