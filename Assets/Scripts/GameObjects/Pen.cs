using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Pen : EventSubscriber, ILoggable, 
    OnPenEnterCanvas.IHandler, OnPenExitCanvas.IHandler, 
    OnVocabItemWriteState.IHandler, OnVocabItemMenuState.IHandler,
    OnVocabItemIdleState.IHandler, OnLetterPredicted.IHandler {
    /// <summary>
    /// The "ink" particle system.
    /// </summary>
    [SerializeField] private ParticleSystem penInkParticle;
    
    /// <summary>
    /// How long should the pen wait to log its position again?
    /// </summary>
    [Rename("Log Interval (secs)")]
    [Tooltip("How long should the pen wait to log its position again?")]
    [SerializeField] private float logTime = 0.25f;

    /// <summary>
    /// Humans will more or less have shaky hands.
    /// How far away from the last recorded position can the pen be until it's counted as being in a new position?
    /// </summary>
    [Tooltip("How far away from the last recorded position can the pen be until it's counted as being in a new position?")]
    [SerializeField] private float errorBounds = 0.002f;
    
    /// <summary>
    /// Controller haptics intensity.
    /// </summary>
    [SerializeField] private float _amplitude = 0.25f;
    
    private XRBaseController _controller;
    
    private bool _onCanvas = false;
    private bool _wroteSomething = false;

    private bool _canLog;
    private Coroutine logCoroutine;
    private Coroutine hapticsCoroutine;
    private Vector3 penLastPosition;
    private Quaternion penLastRotation;

    protected override void Start() {
        base.Start();
        penInkParticle.Stop();
    }

    public void AddTriggerListener(InputActionReference trigger) {
        trigger.action.started += x => StartInk();
        trigger.action.canceled += x => StopInk();
    }

    private void StartInk() {
        LogEvent("StartInk called", LogLevel.Debug);
        if (!_onCanvas) return;
        
        penInkParticle.Play();
        LogEvent("Ink activated", LogLevel.Debug);
        
        if (!_wroteSomething) {
            _wroteSomething = true;
            EventBus.Instance.OnPenWrittenSomething.Invoke(this);
        }

        LogPenPosition(); // position when button is pressed
        logCoroutine = StartCoroutine(IntervalLogPenPosition());
    }

    private void StopInk() {
        LogEvent("StopInk called", LogLevel.Debug);
        if (!_onCanvas) return;
        penInkParticle.Stop();
        if (logCoroutine != null) StopCoroutine(logCoroutine);
        
        LogPenPosition(); // position when button is let go
    }

    IEnumerator IntervalLogPenPosition() {
        while (true) {
            yield return new WaitForSeconds(logTime);
            LogPenPosition(); // every logTime seconds
        }
    }

    private void LogPenPosition() {
        var penTransform = transform;
        
        Vector3 penPosition = penTransform.position;
        Quaternion penRotation = penTransform.rotation;

        if (IsInSamePosition(penPosition, penLastPosition) 
            || penLastRotation == penRotation) 
            return;

        LogEvent($"{penPosition}; {penRotation}");

        penLastPosition = penPosition;
        penLastRotation = penRotation;
    }

    private bool IsInSamePosition(Vector3 posA, Vector3 posB) {
        return Vector3.Distance(posA, posB) < errorBounds;
    }

    public void SetUpHaptics(XRBaseController controller) {
        _controller = controller;
    }

    private IEnumerator StartHaptics(float amplitude) {
        while (true) {
            _controller.SendHapticImpulse(amplitude, 10f);
            yield return new WaitForSecondsRealtime(0.1f);
        };
    }

    private void StopHaptics() {
        _controller.SendHapticImpulse(0f, 1f);
    }

    void OnPenEnterCanvas.IHandler.OnEvent(LetterCanvas canvas) {
        _onCanvas = true;
        hapticsCoroutine = StartCoroutine(StartHaptics(_amplitude));
    }

    void OnPenExitCanvas.IHandler.OnEvent(LetterCanvas canvas) {
        StopInk();
        if (hapticsCoroutine != null) StopCoroutine(hapticsCoroutine);
        StopHaptics();
        _onCanvas = false;
    }

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string character, int position) {
        _wroteSomething = false;
    }
    
    void OnVocabItemWriteState.IHandler.OnEvent(VocabItem vocabItem) {
        _canLog = true;
    }

    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        _canLog = false;
    }
    
    void OnVocabItemIdleState.IHandler.OnEvent(VocabItem vocabItem) {
        _canLog = false;
    }
    
    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        if (_canLog)
            EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
