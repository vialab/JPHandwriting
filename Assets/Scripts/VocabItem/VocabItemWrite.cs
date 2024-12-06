using System.Collections.Generic;
using UnityEngine;

public class VocabItemWrite : EventSubscriber, 
    OnLetterPredicted.IHandler {

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

    private VocabItem _parentVocabItem;
    private string _japaneseText;

    private void Awake() {
        _vocabUI = GetComponent<VocabUI>();
        _parentVocabItem = transform.parent.parent.GetComponent<VocabItem>();
        _japaneseText = _parentVocabItem.JPName;
        charPosition = 0;
    }
    
    // ==============
    // Actual methods
    // ==============
     
    private bool IsActiveVocabItem(VocabItem vocabItem) {
        return (vocabItem == _parentVocabItem && Game.Instance.CurrentVocabItem == _parentVocabItem);
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
        lastChar = Hiragana.TryToggleSmall(lastChar);
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

    private void CheckAddCharacter(string character, bool isMatch) {
        if (!isMatch) return;
        
        _writtenText.Add(character);
        charPosition++;
        UpdateUI();
    }

    private void CompareCharacter(string writtenChar, int position) {
        if (position >= _japaneseText.Length) return;

        var charInCurrentPosition = _japaneseText[position].ToString();

        if (writtenChar == charInCurrentPosition) {
            AddCharacter(writtenChar);
            SetNextCharacterTracing();
        }
        else if (Hiragana.TryToggleSmall(writtenChar) == charInCurrentPosition) {
            AddCharacter(charInCurrentPosition);
            SetNextCharacterTracing();
        } 
        
        // else do nothing
    }

    private void SetNextCharacterTracing() {
        var nextChar = "";

        if (charPosition < _japaneseText.Length) {
            nextChar = _japaneseText[charPosition].ToString();

            if (Hiragana.IsSmall(nextChar))
                nextChar = Hiragana.TryToggleSmall(nextChar);
        }
        
        EventBus.Instance.OnNextTracedLetter.Invoke(_parentVocabItem, nextChar);
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
            _parentVocabItem,
            string.Join("", _writtenText.ToArray())
        );
        
        Hide();
    }

    
    // ===============
    // Event listeners
    // ===============

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string writtenChar, int position) {
        // if this item isn't the active item, do nothing
        if (!IsActiveVocabItem(vocabItem)) return;

        // check if tracing is enabled
        var tracingEnabled = _parentVocabItem.TracingEnabled;
        
        // if so, compare letters then add; if not, add
        if (!tracingEnabled) {
            AddCharacter(writtenChar);
        } else {
            CompareCharacter(writtenChar, position);
        }
    }
}
