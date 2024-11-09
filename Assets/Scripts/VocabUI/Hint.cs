using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hint : VocabUI {
    public void ShowWithHint(string kanaName, string romajiText) {
        SetUIText(romajiText);
        Show();
        LogEvent($"Hint accessed for {kanaName}");
    }
}
