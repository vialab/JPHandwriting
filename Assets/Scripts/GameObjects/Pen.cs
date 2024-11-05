using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pen : EventSubscriber, ILoggable, OnPenEnterCanvas.IHandler, OnPenExitCanvas.IHandler {
    /// <summary>
    /// The "ink" particle system.
    /// </summary>
    [SerializeField] private ParticleSystem penInkParticle;
    
    /// <summary>
    /// How long should the pen wait to log its position again?
    /// </summary>
    [SerializeField] private float logTime = 0.5f;

    /// <summary>
    /// Humans will more or less have shaky hands.
    /// How much 
    /// </summary>
    [SerializeField] private float errorBounds = 0.1f;
    
    private bool _onCanvas = false;
    private XRGrabInteractable _grabInteractable;

    private Coroutine logCoroutine;
    private Vector3 penLastPosition;
    private Quaternion penLastRotation;

    private void Awake() {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    protected override void Start() {
        base.Start();
        penInkParticle.Stop();
        
        _grabInteractable.activated.AddListener(x => StartInk());
        _grabInteractable.activated.AddListener(x => StopInk());
    }

    private void StartInk() {
        if (!_onCanvas) return;
        penInkParticle.Play();
        
        LogPenPosition(); // position when button is pressed
        logCoroutine = StartCoroutine(IntervalLogPenPosition());
    }

    private void StopInk() {
        penInkParticle.Stop();
        StopCoroutine(logCoroutine);
        
        LogPenPosition(); // position when button is let go
    }

    IEnumerator IntervalLogPenPosition() {
        while (true) {
            yield return new WaitForSeconds(logTime);
            LogPenPosition(); // every logTime seconds
        }
    }

    private void LogPenPosition() {
        Vector3 penPosition = transform.position;
        Quaternion penRotation = transform.rotation;

        if (IsInSamePosition(penPosition, penLastPosition) 
            || penLastRotation == penRotation) 
            return;

        LogEvent($"Pen pos/rot: {penPosition}; {penRotation}");

        penLastPosition = penPosition;
        penLastRotation = penRotation;
    }

    private bool IsInSamePosition(Vector3 posA, Vector3 posB) {
        return Vector3.Distance(posA, posB) < errorBounds;
    }

    void OnPenEnterCanvas.IHandler.OnEvent(LetterCanvas canvas) {
        _onCanvas = true;
    }

    void OnPenExitCanvas.IHandler.OnEvent(LetterCanvas canvas) {
        _onCanvas = false;
    }
    
    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}
