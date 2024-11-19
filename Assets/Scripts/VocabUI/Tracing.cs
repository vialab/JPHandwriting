public class Tracing : VocabUI {
    public void SetCharacter(string character) {
        SetUIText(character);
        
        LogEvent($"Character to trace is now {character}");
    }
}
