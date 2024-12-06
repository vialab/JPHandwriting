using System.Collections;
using System.Linq;
using UnityEngine;
using ExtensionMethods;
using UnityEditor.PackageManager;

public class VocabItem : SerializableEventSubscriber<VocabItem>, ILoggable, 
    OnVocabItemFinishGuess.IHandler {
    // =====================
    // MARK: Vocab item properties
    // =====================

    /// <summary>
    /// The vocabulary word as it's known in English.
    /// Also known as SerializableEventSubscriber.Type.
    /// </summary>
    protected string englishName = "";

    /// <summary>
    /// The vocabulary as it's known in Japanese.
    /// The writing is in Hiragana only.
    /// </summary>
    [SerializeField] protected string japaneseName;

    /// <summary>
    /// A clip of how the vocabulary is pronounced in Japanese.
    /// </summary>
    [SerializeField] protected AudioClip pronunciationClip;

    // Getters for vocab properties
    public string ENName => englishName;
    public string JPName => japaneseName;


    // =============
    // MARK: State objects
    // =============

    [SerializeField] protected VocabInfo vocabInfoUIObject;

    /// <summary>
    /// The object that controls the UI in Menu state. 
    /// </summary>
    [SerializeField] protected Menu menuStateObject;

    /// <summary>
    /// The object that controls the UI in Writing state.
    /// </summary>
    [SerializeField] protected VocabItemWrite writeStateObject;

    public int CurrentPosition => writeStateObject.CharPosition;
    public string CharAtCurrentPosition => japaneseName[writeStateObject.CharPosition].ToString();


    // ==========
    // MARK: UI objects
    // ==========

    /// <summary>
    /// The coloured outline of the object.
    /// Green if the user has learned it, red otherwise.
    /// </summary>
    private Outline _outline;


    // ==========================
    // MARK: WriteState-specific things
    // ==========================

    /// <summary>
    /// Whether to show the character to trace over or not.
    /// </summary>
    [Rename("Toggle Tracing")]
    public bool enableTracing = false;

    public bool TracingEnabled => enableTracing;


    // =====================
    // MARK: Other item properties
    // =====================

    private VocabularyState _vocabularyState = VocabularyState.NotLearned;
    private ObjectUIState _objectUIState = ObjectUIState.Idle;

    private bool IsActiveVocabItem(VocabItem vocabItem) {
        return vocabItem == this && Game.Instance.CurrentVocabItem == this;
    }


    // ===============
    // MARK: Class overrides
    // ===============

    public override string ToString() {
        return $"{Type} ({(_vocabularyState == VocabularyState.Learned ? "" : "not ")}learned)";
    }

    
    // ===========================
    // MARK: Unity MonoBehaviour methods
    // ===========================

    protected void Awake() {
        _outline = gameObject.AddComponent<Outline>();
        englishName = Type;
    }

    protected override void Start() {
        base.Start();
        
        // Vocab info stuff
        vocabInfoUIObject.SetClip(pronunciationClip);
        vocabInfoUIObject.SetUIText(englishName);
        vocabInfoUIObject.SetTracingIndicator(enableTracing);
        vocabInfoUIObject.Hide();
        
        // Menu state setup
        menuStateObject.Hide();

        // Hiding everything else
        writeStateObject.Hide();

        // Other setup
        SetOutline();

        LogEvent($"{Type} instantiated", LogLevel.Debug);
    }


    // ==================
    // MARK: UI display methods
    // ==================

    public void ShowMenuState() {
        // disable everything from write state
        if (_objectUIState == ObjectUIState.Writing) {
            writeStateObject.Hide();
        }

        // change state
        _objectUIState = ObjectUIState.Menu;
        EventBus.Instance.OnVocabItemMenuState.Invoke(this);

        // show menu
        EnableSelectedOutline();
        menuStateObject.Show();
        vocabInfoUIObject.Show();
        LogEvent("Vocabulary entered Menu state");
    }

    public void ShowWriteState() {
        // disable everything from menu state
        if (_objectUIState == ObjectUIState.Menu) {
            menuStateObject.Hide(); // probably redundant check
        }

        // change state
        _objectUIState = ObjectUIState.Writing;
        EventBus.Instance.OnVocabItemWriteState.Invoke(this);

        // show write state UI
        writeStateObject.Show();
        vocabInfoUIObject.Show();
        LogEvent("Vocabulary entered Write state");
    }

    public void ShowIdleState() {
        _objectUIState = ObjectUIState.Idle;
        EventBus.Instance.OnVocabItemIdleState.Invoke(this);

        menuStateObject.Hide();
        writeStateObject.Hide(); 
        vocabInfoUIObject.Hide();
    }


    // =======================
    // MARK: Outline display methods 
    // =======================

    /// <summary>
    /// Outline setup on game start.
    /// </summary>
    private void SetOutline() {
        _outline.enabled = true;
        _outline.OutlineWidth = 5f;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.OutlineColor = Color.white;
    }

    /// <summary>
    /// Disable any changes to the outline.
    /// </summary>
    public void DisableOutlineChanges() {
        if (_objectUIState == ObjectUIState.Menu) return;

        _outline.OutlineColor = Color.white;
    }

    /// <summary>
    /// Change the outline colour when the item is in menu state.
    /// </summary>
    private void EnableSelectedOutline() {
        _outline.OutlineColor = _vocabularyState == VocabularyState.Learned ? Color.green : Color.red;
    }

    /// <summary>
    /// Change the outline colour when the outline is being hovered over.
    /// </summary>
    public void EnableHoverOutline() {
        _outline.OutlineColor = Color.yellow;
    }
    public void EnableGrabHoverOutline() {
        _outline.OutlineColor = Color.blue;
    }


    // ======================
    // MARK: Event listener methods
    // ======================

    void OnVocabItemFinishGuess.IHandler.OnEvent(VocabItem vocabItem, string guess) {
        // if this item isn't the active item, do nothing
        if (!IsActiveVocabItem(vocabItem)) return;

        Wordle(guess);
    }


    // ==================
    // MARK: WriteState methods 
    // ==================

    /// <summary>
    /// Checks if the user's guess matches the japaneseName field.
    /// If not, just like Wordle, shows characters:
    /// - the user guessed correctly, in the right position
    /// - the user guessed correctly, but in the wrong position
    /// - the user got totally wrong
    /// </summary>
    /// <param name="guess">The user's written guess.</param>
    private void Wordle(string guess) {
        // Word is correct; return early
        if (guess == japaneseName) {
            LogEvent("Word is correct");

            _vocabularyState = VocabularyState.Learned;
            EventBus.Instance.OnVocabItemLearned.Invoke(this);
            
            EventBus.Instance.ToastNotification.Invoke(this, "Correct!<br>Good job!");

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
        for (var pos = 0; pos < shorterWord.Length; pos++) {
            var currentChar = isGuessShorter ? shorterWord[pos].ToString() : longerWord[pos].ToString();

            if (currentChar == japaneseName[pos].ToString()) {
                // Correct character, correct position
                guessUIText += currentChar.WithColor("green");
            }
            else if (japaneseName.Contains(Hiragana.TryToggleSmall(currentChar))) {
                // Correct character, wrong position
                // or user submitted small when they meant large, or vice versa
                guessUIText += currentChar.WithColor("yellow");
            }
            else {
                // Wrong character, wrong position
                guessUIText += currentChar;
            }
        }

        LogEvent($"User was close, their guess: {guess}");
        
        EventBus.Instance.ToastNotification.Invoke(this, $"Try again!<br>Your guess: {guessUIText}");
    }


    // ==========
    // MARK: Log method 
    // ==========

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}