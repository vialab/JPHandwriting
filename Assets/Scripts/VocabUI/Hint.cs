using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hint : VocabUI {
    public void ShowWithHint(string kanaName, string romajiText) {
        SetUIText(romajiText);
        Show();
        LogEvent($"Hint accessed for {kanaName}");
    }

    protected override void LookAtPlayer() {
        base.LookAtPlayer();
        transform.forward *= -1f;
    }
}
