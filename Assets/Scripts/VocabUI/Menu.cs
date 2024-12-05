using UnityEngine;

// Not inheriting VocabUI because no text
public class Menu : MonoBehaviour {
    public virtual void Show() {
        gameObject.SetActive(true);
    }

    public virtual void Hide() {
        gameObject.SetActive(false);
    }
}