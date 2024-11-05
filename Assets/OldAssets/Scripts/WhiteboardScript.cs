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
    private bool isTouching = false;
    private bool isFinishing = false;
    private XRGrabInteractable _xrGrabInteractable;

    private Vector3 lastPosition = Vector3.zero; 
    private Quaternion lastRotation = Quaternion.identity;

    private void Awake() {
    }

    private void Start() {
    }

    private void Update() {
        if (isFinishing) {
            // Why not a coroutine? Those are confusing
            timeOffCanvas += Time.deltaTime;

            if (timeOffCanvas > timeUntilImageCreate) {
                Debug.Log("It's been a few seconds");
                timeOffCanvas = 0f;

                isFinishing = false;
                
                // hit up the image exporter
                ActivityLogger.Instance.LogEvent("Exporting canvas as image", this);
                OnHandwritingFinish?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == markerObject.name) {
            ActivityLogger.Instance.LogEvent("Pen collided with whiteboard", this);
        }
    }

    private void OnTriggerStay(Collider other) {
        // Debug.Log(other.name);

        // if the object colliding is a marker object 
        if (other.name != markerObject.name) {
            return;
        }

        isTouching = true;

        // sanity check
        isFinishing = false;
        timeOffCanvas = 0f;

        // enable ink
        OnMarkerInteraction?.Invoke(this, new MarkerInteractionArgs {
            isTouching = isTouching
        });

        // TODO: pen coordinates
        if (!isTouching) {
            return;
        }
        
        // TODO: have this lag... less
        Vector3 penPosition = other.transform.position;
        Quaternion penRotation = other.transform.rotation;

        if (penRotation != lastRotation && penPosition != lastPosition) {
            ActivityLogger.Instance.LogEvent($"Pen position: {other.transform.position}", this);
            ActivityLogger.Instance.LogEvent($"Pen rotation: {other.transform.rotation}", this);
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
            ActivityLogger.Instance.LogEvent("Pen no longer on whiteboard", this);

        }
    }
}
