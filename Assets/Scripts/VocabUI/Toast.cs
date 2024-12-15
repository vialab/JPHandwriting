using System.Collections;
using UnityEngine;

public class Toast : VocabUI, ToastNotification.IHandler {
    [SerializeField] private float toastDisplayTime = 3f;
    
    public IEnumerator ShowToast(string message) {
        SetUIText(message);
        Show();

        yield return new WaitForSeconds(toastDisplayTime);
        
        Hide();
    }

    void ToastNotification.IHandler.OnEvent(Object obj, string text) {
        StartCoroutine(ShowToast(text));
    }
}
