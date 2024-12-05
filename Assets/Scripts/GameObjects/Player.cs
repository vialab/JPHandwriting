using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public enum WritingHand {
    Right, Left,
}

public class Player : EventSubscriber, ILoggable, OnVocabItemWriteState.IHandler, OnVocabItemMenuState.IHandler, OnLetterPredicted.IHandler {
    [Rename("User's Dominant Hand")]
    [SerializeField] private WritingHand hand = WritingHand.Right;

    [SerializeField] private Vector3 _leftPointerFingertip; // Vector3(-0.0177999996,-0.0115,0.0595000014)
    [SerializeField] private Vector3 _rightPointerFingertip; // Vector3(0.00879999995,-0.0195000004,0.0590000004)
    
    /// <summary>
    /// How long to wait for the canvas clearing cube to clear the canvas.
    /// </summary>
    [Rename("Eraser Cube Wait Time")]
    [SerializeField] protected float waitTimeSeconds = 0.15f;

    private ParticleSystem _penInkParticle;
    private LetterCanvas _canvas;
    private GameObject _canvasCube;
    private VocabItem _currentItem;
    private ObjectUIState _currentObjectUIState = ObjectUIState.Idle;

    private void Awake() {
        _penInkParticle = GetComponentInChildren<ParticleSystem>();

    }

    protected override void Start() {
        base.Start();

        // TODO: put pen material on user's <_hand> finger
        _penInkParticle.transform.position = hand == WritingHand.Left ? _leftPointerFingertip : _rightPointerFingertip;
        
        LogEvent($"Pen ink position is {_penInkParticle.transform.position} ({hand})", LogLevel.Debug);
    }
    
    /// <summary>
    /// "Clears" the canvas.
    /// Actually, spawns a big white cube for waitTimeSeconds to overwrite the contents of the canvas with pure white,
    /// and then despawns and moves the cube out of the way.
    /// </summary>
    private IEnumerator ClearCanvas() {
        var cubePosition = _canvas.transform.localPosition;
        _canvasCube.SetActive(true);

        LogEvent("Canvas clearing object appeared");

        // Move the cube up a bit
        _canvasCube.transform.localPosition = new Vector3(
            cubePosition.x,
            cubePosition.y + 0.005f,
            cubePosition.z
        );

        // wait for it
        yield return new WaitForSecondsRealtime(waitTimeSeconds);

        // and move it out of the way
        // the pen ink won't show if it's just disabled
        _canvasCube.transform.Translate(Vector3.down * 10f, transform.parent);
        _canvasCube.SetActive(false);
        LogEvent("Canvas clearing object disappeared");
    }
    
    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        _currentObjectUIState = ObjectUIState.Menu;

        _currentItem = vocabItem;
    }

    void OnVocabItemWriteState.IHandler.OnEvent(VocabItem vocabItem) {
        if (_currentItem == vocabItem)
            _currentObjectUIState = ObjectUIState.Writing;
        
        // TODO: spawn canvas in front of player
        // TODO: activate ink material
    }

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string writtenChar, int position) {
        StartCoroutine(ClearCanvas());
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
