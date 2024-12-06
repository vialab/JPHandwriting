using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum WritingHand {
    Right, Left,
}

[System.Serializable]
public class HandInformation {
    public InputActionReference handTrigger;
    public GameObject directInteractor;
    public GameObject rayInteractor;

    public void DisableInteractors() {
        directInteractor.SetActive(false);
        rayInteractor.SetActive(false);
    }
}

public class Player : EventSubscriber, ILoggable, 
    OnVocabItemWriteState.IHandler, OnVocabItemMenuState.IHandler, 
    OnLetterPredicted.IHandler {
    [Rename("User's Dominant Hand")]
    [SerializeField] private WritingHand hand = WritingHand.Right;

    [SerializeField] private GameObject _fullCanvas;
    [SerializeField] private GameObject canvasCube;
    [SerializeField] private Pen _pen;

    [SerializeField] private HandInformation leftHandInfo;
    [SerializeField] private HandInformation rightHandInfo;
    
    /// <summary>
    /// How long to wait for the canvas clearing cube to clear the canvas.
    /// </summary>
    [Rename("Eraser Cube Wait Time")]
    [SerializeField] protected float waitTimeSeconds = 0.15f;
    
    private Camera mainCam;
    private VocabItem _currentItem;

    private void Awake() {
        // _pen = GetComponentInChildren<Pen>();
    }

    protected override void Start() {
        base.Start();
        mainCam = Game.Instance.MainCam;
        
        SetUpFingerWriting();

        _fullCanvas.SetActive(false);
    }

    private void SetUpFingerWriting() {
        var handInfo = hand == WritingHand.Left ? leftHandInfo : rightHandInfo;
        var otherHand = hand == WritingHand.Left ? rightHandInfo : leftHandInfo;
        var fingerRayTransform = handInfo.rayInteractor.transform;

        // put pen material on user's <_hand> finger
        _pen.transform.SetParent(fingerRayTransform.parent);
        _pen.transform.localPosition = fingerRayTransform.localPosition + Vector3.forward * 0.05f;

        // set trigger of dominant hand to start ink
        _pen.AddTriggerListener(handInfo.handTrigger);
        LogEvent($"Pen ink position is {_pen.transform.position} ({hand})", LogLevel.Debug);
        
        // disable other hand
        otherHand.DisableInteractors();
    }

    /// <summary>
    /// Moves canvas to be in front of the player when the vocab item reaches menu state.
    /// </summary>
    private void MoveCanvas() {
        var mainCamTransform = mainCam.transform;
        var mainCamForward = mainCamTransform.forward;
        var mainCamPos = mainCamTransform.position;

        _fullCanvas.transform.position = mainCamPos +
                                     new Vector3(mainCamForward.x, mainCamForward.y - 0.5f, mainCamForward.z).normalized *
                                     0.5f;

        _fullCanvas.transform.LookAt(new Vector3(mainCamPos.x, _fullCanvas.transform.position.y, mainCamPos.z));
        _fullCanvas.transform.forward *= -1;
    }
    
    /// <summary>
    /// "Clears" the canvas.
    /// Actually, spawns a big white cube for waitTimeSeconds to overwrite the contents of the canvas with pure white,
    /// and then despawns and moves the cube out of the way.
    /// </summary>
    private IEnumerator ClearCanvas() {
        var cubePosition = _fullCanvas.transform.localPosition;
        canvasCube.SetActive(true);

        LogEvent("Canvas clearing object appeared");

        // Move the cube up a bit
        canvasCube.transform.localPosition = new Vector3(
            cubePosition.x,
            cubePosition.y + 0.005f,
            cubePosition.z
        );

        // wait for it
        yield return new WaitForSecondsRealtime(waitTimeSeconds);

        // and move it out of the way
        // the pen ink won't show if it's just disabled
        canvasCube.transform.Translate(Vector3.down * 10f, transform.parent);
        canvasCube.SetActive(false);
        LogEvent("Canvas clearing object disappeared");
    }

    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        _currentItem = vocabItem;
        // TODO: move eraser in same position as canvas here to clear it
    }

    void OnVocabItemWriteState.IHandler.OnEvent(VocabItem vocabItem) {
        if (_currentItem != vocabItem) return;
        
        MoveCanvas();
        _fullCanvas.SetActive(true);
    }
    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string writtenChar, int position) {
        StartCoroutine(ClearCanvas());
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
