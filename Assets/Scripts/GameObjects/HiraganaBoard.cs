using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class HiraganaBoard : MonoBehaviour, ILoggable {
    private Camera mainCam;
    private Plane[] frustumPlanes;
    private Collider visionCollider;

    private bool isInView;
    private bool wasInView; // I know I'll have to do this

    private void Awake() {
        mainCam = Camera.main;
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCam);
        visionCollider = GetComponentInChildren<Collider>();
    }

    // TODO: this is not actually working and it'll always be in user's view
    private void Update() {
        isInView = GeometryUtility.TestPlanesAABB(frustumPlanes, visionCollider.bounds);
        if (wasInView != isInView) {
            LogEvent($"Stroke order board {(isInView ? "entered" : "exited")} user's view");
            wasInView = isInView;
        }
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
