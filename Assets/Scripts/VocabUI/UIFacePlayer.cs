using UnityEngine;
using UnityEngine.Serialization;

public class UIFacePlayer : MonoBehaviour {
    private Camera mainCam;
    
    private void Start() {
        mainCam = Game.Instance.MainCam;
    }
    
    private void Update() {
        if (gameObject.activeInHierarchy) LookAtPlayer();
    }
    
    protected virtual void LookAtPlayer() {
        var mainCamPosition = mainCam.transform.position;

        transform.LookAt(new Vector3(mainCamPosition.x, transform.position.y, mainCamPosition.z));
        transform.forward *= -1f;
    }
}
