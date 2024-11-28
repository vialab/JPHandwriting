using System.Collections;
using System.Collections.Generic;
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

    // TODO: hopefully I don't need to do anything more
    private void Update() {
        isInView = GeometryUtility.TestPlanesAABB(frustumPlanes, visionCollider.bounds);
        if (wasInView != isInView) {
            LogEvent($"Stroke order board {(isInView ? "entered" : "exited")} user's view");
            wasInView = isInView;
        }
    }

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}
