using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VocabItemWrite : EventSubscriber, OnLetterPredicted.IHandler {
    /// <summary>
    /// Whether to show the character to trace over or not.
    /// </summary>
    [Rename("Toggle Tracing")]
    [SerializeField] private bool enableTracing;

    /// <summary>
    /// Assume VocabItemWrite and VocabUI are attached to the same object.
    /// </summary>
    private VocabUI _vocabUI;
    
    /// <summary>
    /// A Stack representing the characters a user has written.
    /// </summary>
    private readonly Stack<string> _writtenText = new();

    private void Awake() {
        _vocabUI = GetComponent<VocabUI>();
    }

    public void Show() {
        gameObject.SetActive(true);
        _vocabUI.Show();
    }

    public void Hide() {
        gameObject.SetActive(false);
        _writtenText.Clear(); // no saving progress for user, they have to retry
        _vocabUI.Hide();
    }
    public void ToggleChiisai() {
        string lastChar = _writtenText.Pop();

        if (Hiragana.CHIISAI.TryGetValue(lastChar, out var value)) {
            lastChar = value;
        } else if (Hiragana.CHIISAI.ContainsValue(lastChar)) {
            lastChar = Hiragana.CHIISAI.FirstOrDefault(x => x.Value.Equals(lastChar)).Key;
        }
        
        AddCharacter(lastChar);
    }
    
    private void UpdateUI() {
        _vocabUI.SetUIText(string.Join("", _writtenText));
    }

    private void AddCharacter(string character) {
        _writtenText.Push(character);
        UpdateUI();
    }
    
    // ================
    // Button functions
    // ================
    
    /// <summary>
    /// Undoes the last written character.
    /// </summary>
    public void RemoveCharacter() {
        _writtenText.Pop();
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
