using System.Collections;
using ToastNotification;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public enum WritingHand {
    Right, Left,
}

[System.Serializable]
public class HandInformation {
    public InputActionReference handTrigger;
    public GameObject directInteractor;
    public GameObject rayInteractor;
    public XRBaseController controller;

    public void DisableInteractors() {
        directInteractor.SetActive(false);
        rayInteractor.SetActive(false);
    }
}

public class Player : EventSubscriber, ILoggable, 
    OnVocabItemWriteState.IHandler, OnVocabItemMenuState.IHandler, 
    OnLetterPredicted.IHandler, ToastNotification.IHandler {
    [Rename("User's Dominant Hand")]
    [SerializeField] private WritingHand hand = WritingHand.Right;

    [SerializeField] private static GameObject _fullCanvas;
    [SerializeField] private GameObject canvasCube;
    [SerializeField] private Pen _pen;
    [SerializeField] private Toast _toastUI;

    [SerializeField] private HandInformation leftHandInfo;
    [SerializeField] private HandInformation rightHandInfo;
    
    /// <summary>
    /// How long to wait for the canvas clearing cube to clear the canvas.
    /// </summary>
    [Rename("Eraser Cube Wait Time")]
    [SerializeField] protected float waitTimeSeconds = 0.15f;
    
    [SerializeField] private Camera _mainCam;
    private VocabItem _currentItem;

    private void Awake() {
        // _pen = GetComponentInChildren<Pen>();
    }

    protected override void Start() {
        base.Start();
        
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
        
        // give pen reference to controller for haptics
        _pen.SetUpHaptics(handInfo.controller);
        
        // disable other hand
        otherHand.DisableInteractors();
    }
    
    private void MoveObjectFacingPlayer(GameObject obj) {
        var mainCamTransform = _mainCam.transform;
        var mainCamForward = mainCamTransform.forward;
        var mainCamPos = mainCamTransform.position;

        obj.transform.position = mainCamPos +
                                     new Vector3(mainCamForward.x, mainCamForward.y - 0.5f, mainCamForward.z).normalized *
                                     0.5f;

        obj.transform.LookAt(new Vector3(mainCamPos.x, _fullCanvas.transform.position.y, mainCamPos.z));
        obj.transform.forward *= -1;
    }

    /// <summary>
    /// Moves canvas to be in front of the player when the vocab item reaches menu state.
    /// </summary>
    private void MoveCanvas() {
        MoveObjectFacingPlayer(_fullCanvas);
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

    void ToastNotification.IHandler.OnEvent(Object obj, string message) {
        _toastUI.SetUIText(message);
        MoveObjectFacingPlayer(_toastUI.gameObject);
        _toastUI.Show();
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
