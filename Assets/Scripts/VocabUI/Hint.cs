using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hint : VocabUI {
    public override void Show() {
        base.Show();
        LogEvent($"Hint accessed for {UIText.text}");
    }
}
