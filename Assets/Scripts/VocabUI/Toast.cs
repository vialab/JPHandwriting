using System.Collections;
using UnityEngine;

public class Toast : VocabUI {
    [SerializeField] private float toastDisplayTime = 3f;
    
    public IEnumerator ShowToast(string message) {
        SetUIText(message);
        Show();

        yield return new WaitForSeconds(toastDisplayTime);
        
        Hide();
    }
}
