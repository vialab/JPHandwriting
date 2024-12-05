public class Tracing : VocabUI, OnNextTracedLetter.IHandler {
    protected override void LookAtPlayer() {
        // don't do anything
    }

    private void SetCharacter(string character) {
        SetUIText(character);
        
        LogEvent($"Character to trace is now {character}");
    }

    void OnNextTracedLetter.IHandler.OnEvent(VocabItem vocabItem, string character) {
        SetCharacter(character);
    }
}
