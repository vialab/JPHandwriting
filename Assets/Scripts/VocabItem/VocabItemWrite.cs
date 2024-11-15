using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VocabItemWrite : EventSubscriber, OnLetterPredicted.IHandler {

    /// <summary>
    /// Assume VocabItemWrite and VocabUI are attached to the same object.
    /// </summary>
    private VocabUI _vocabUI;
    
    /// <summary>
    /// A Stack representing the characters a user has written.
    /// </summary>
    private readonly List<string> _writtenText = new();
    public List<string> WrittenText => _writtenText;

    /// <summary>
    /// How many characters the user has written, minus 1.
    /// </summary>
    private int charPosition = 0;
    public int CharPosition => charPosition;

    private void Awake() {
        _vocabUI = GetComponent<VocabUI>();
        charPosition = 0;
    }

    public void Show() {
        gameObject.SetActive(true);
        _vocabUI.Show();
    }

    public void Hide() {
        gameObject.SetActive(false);
        
        // no saving progress for user, they have to retry
        _writtenText.Clear(); 
        charPosition = 0;
       
        _vocabUI.Hide();
    }
    public void ToggleChiisai() {
        string lastChar = _writtenText[^1];
        
        RemoveCharacter();

        Hiragana.TryToggleSmall(lastChar);
        
        AddCharacter(lastChar);
    }
    
    private void UpdateUI() {
        _vocabUI.SetUIText(string.Join("", _writtenText));
    }

    private void AddCharacter(string character) {
        _writtenText.Add(character);
        charPosition++;
        UpdateUI();
    }
    
    // ================
    // Button functions
    // ================
    
    /// <summary>
    /// Undoes the last written character.
    /// </summary>
    public void RemoveCharacter() {
        _writtenText.RemoveAt(_writtenText.Count - 1);
        charPosition--;
        UpdateUI();
    }
    
    /// <summary>
    /// Triggers the Vocab item to check whether the user's written guess matches
    /// its Japanese name.
    /// </summary>
    public void SubmitGuess() {
        EventBus.Instance.OnVocabItemFinishGuess.Invoke(
            transform.parent.GetComponent<VocabItem>(),
            string.Join("", _writtenText.ToArray())
        );
        
        Hide();
    }
    
    // ===============
    // Event listeners
    // ===============

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string character) {
        AddCharacter(character);
    }
}
