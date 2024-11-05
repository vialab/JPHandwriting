using System;
using TMPro;
using UnityEngine;
using ExtensionMethods;

public class VocabItem : EventSubscriber, ILoggable, OnVocabItemFinishGuess.IHandler {
    // ==========================================
    // Vocab item properties (probably from JSON)
    // ==========================================
    
    /// <summary>
    /// The vocabulary word as it's known in English.
    /// </summary>
    [SerializeField] private string englishName;
    
    /// <summary>
    /// The vocabulary as it's known in Japanese.
    /// The writing is in Hiragana only.
    /// </summary>
    [SerializeField] private string japaneseName;
    
    /// <summary>
    /// A clip of how the vocabulary is pronounced in Japanese.
    /// </summary>
    [SerializeField] private AudioClip pronunciationClip;

    /// <summary>
    /// Whether to show the character to trace over or not.
    /// </summary>
    [SerializeField] private bool enableCheating;
    
    // Getters for vocab properties
    public string ENName => englishName;
    public string JPName => japaneseName;
    
    // =============================================
    // Spawn points for pen and canvas on WriteState
    // =============================================
    
    /// <summary>
    /// Where the pen should spawn when the item is in Write state. 
    /// </summary>
    [SerializeField] private Transform penSpawnPoint;
    
    /// <summary>
    /// Where the canvas should spawn when the item is in Write state.
    /// </summary>
    [SerializeField] private Transform canvasSpawnPoint;

    // =============
    // State objects
    // =============
    
    /// <summary>
    /// The object that controls the UI in Menu state. 
    /// </summary>
    [SerializeField] private Menu menuStateObject;
    
    /// <summary>
    /// The object that controls the UI in Writing state.
    /// </summary>
    [SerializeField] private VocabItemWrite writeStateObject;
    
    // ==========
    // UI objects
    // ==========
    
    /// <summary>
    /// The object that controls the "toast" notification.
    /// </summary>
    [SerializeField] private Toast toastObject;
    
    /// <summary>
    /// The coloured outline of the object.
    /// Green if the user has learned it, red otherwise.
    /// </summary>
    private Outline _outline;
    
    // =====================
    // Other item properties
    // =====================
    // TODO: load from user config
    
    private VocabularyState _vocabularyState = VocabularyState.NotLearned;
    private ObjectUIState _objectUIState = ObjectUIState.Idle;

    // ===========================
    // Unity MonoBehaviour methods
    // ===========================
    private void Awake() {
        menuStateObject.Hide();
        writeStateObject.Hide();
        toastObject.Hide();
        
        _outline = gameObject.AddComponent<Outline>();
    }

    protected override void Start() {
        base.Start();
        
        SetOutline();
        menuStateObject.SetUIText(englishName);
    }

    private void SetOutline() {
        _outline.enabled = false;
        _outline.OutlineWidth = 5f;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
    }
    
    // ==========================
    // UI/outline display methods
    // ==========================

    
    private void DisplayToast(string message) {
        StartCoroutine(toastObject.ShowToast(message));
    }

    public void ShowMenuState() {
        if (_objectUIState == ObjectUIState.Writing) writeStateObject.Hide();
        
        _objectUIState = ObjectUIState.Menu;
        EventBus.Instance.OnVocabItemMenuState.Invoke(this);
        
        menuStateObject.Show();
    }

    public void ShowWriteState() {
        if (_objectUIState == ObjectUIState.Menu) menuStateObject.Hide(); // probably redundant check
        
        _objectUIState = ObjectUIState.Writing;
        EventBus.Instance.OnVocabItemWriteState.Invoke(this);
        
        writeStateObject.Show();
    }

    public void HideUI() {
        _objectUIState = ObjectUIState.Idle;
        EventBus.Instance.OnVocabItemIdleState.Invoke(this);
        
        menuStateObject.Hide();
        writeStateObject.Hide(); // In case another VocabItem wants to be focused
    }

    public void DisableOutline() {
        if (_objectUIState == ObjectUIState.Menu) return;

        _outline.enabled = false;
    }

    public void EnableSelectedOutline() {
        _outline.OutlineColor = _vocabularyState == VocabularyState.Learned ? Color.green : Color.red;
        _outline.enabled = true;
    }

    public void EnableHoverOutline() {
        _outline.OutlineColor = Color.white;
    }
    
    // ======================
    // Event listener methods
    // ======================

    void OnVocabItemFinishGuess.IHandler.OnEvent(VocabItem vocabItem, string guess) {
        Wordle(guess);
    }
    
    // ================
    // Helper functions
    // ================
    
    /// <summary>
    /// Checks if the user's guess matches the japaneseName field.
    /// If not, just like Wordle, shows characters:
    /// - the user guessed correctly, in the right position
    /// - the user guessed correctly, but in the wrong position
    /// - the user made up completely
    /// </summary>
    /// <param name="guess">The user's written guess.</param>
    private void Wordle(string guess) {
        // Word is correct; return early
        if (guess == japaneseName) {
            LogEvent("Word is correct");
            DisplayToast("you did! it");
            _vocabularyState = VocabularyState.Learned;
            return;
        }
        
        // Word is incorrect
        var guessUIText = "";
        
        // Check if lengths are the same
        
        // This works if japaneseName is longer/same length as guess
        string shorterWord = guess, longerWord = japaneseName;
        var isGuessShorter = true;
        
        // In case that isn't, here's a failsafe
        if (guess.Length > japaneseName.Length) {
            shorterWord = japaneseName;
            longerWord = guess;
            isGuessShorter = false;
        }
        
        // Loop through shorter word
        // TODO: see if the fact that JP Unicode is often larger than sizeof(char) affects this
        for (var pos = 0; pos < shorterWord.Length; pos++) {
            var currentChar = isGuessShorter ? shorterWord[pos].ToString() : longerWord[pos].ToString();
            
            
            if (currentChar == japaneseName[pos].ToString()) {
                guessUIText += currentChar.WithColor("green");
            } else if (japaneseName.Contains(currentChar)) {
                guessUIText += currentChar.WithColor("yellow");
            } else {
                guessUIText += currentChar;
            }
        }
        
        LogEvent("User was close");
        DisplayToast($"Try again! Here was your guess:<br>{guessUIText}");
    }

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}