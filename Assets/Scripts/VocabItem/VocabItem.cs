using System.Collections;
using UnityEngine;
using ExtensionMethods;

public class VocabItem : EventSubscriber, ILoggable, OnVocabItemFinishGuess.IHandler, OnLetterPredicted.IHandler {
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
    
    // Getters for vocab properties
    public string ENName => englishName;
    public string JPName => japaneseName;
    
    // =============================================
    // Spawn points for pen and canvas on WriteState
    // =============================================
    
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

    public string LetterAtCurrentPosition => writeStateObject.WrittenText.ToArray()[writeStateObject.CharPosition];
    
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

    [SerializeField] private LetterCanvas _canvas;
    [SerializeField] private GameObject _canvasObject;
    [SerializeField] private GameObject _canvasEraserCube;
    
    private VocabularyState _vocabularyState = VocabularyState.NotLearned;
    private ObjectUIState _objectUIState = ObjectUIState.Idle;

    // ===========================
    // Unity MonoBehaviour methods
    // ===========================
    private void Awake() {
        _outline = gameObject.AddComponent<Outline>();
    }

    protected override void Start() {
        base.Start();
        
        menuStateObject.SetClip(pronunciationClip);
        menuStateObject.Hide();
        writeStateObject.Hide();
        _canvas.Hide();
        toastObject.Hide();
        
        SetUpEraserCube();
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
        if (_objectUIState == ObjectUIState.Writing) {
            writeStateObject.Hide();
            StartCoroutine(ClearCanvas());
            DestroyCanvas();
        }
        
        EnableSelectedOutline();
        _objectUIState = ObjectUIState.Menu;
        EventBus.Instance.OnVocabItemMenuState.Invoke(this);
        
        menuStateObject.Show();
    }

    public void ShowWriteState() {
        if (_objectUIState == ObjectUIState.Menu) {
            menuStateObject.Hide(); // probably redundant check
            DisableOutline();
        }
        
        _objectUIState = ObjectUIState.Writing;
        EventBus.Instance.OnVocabItemWriteState.Invoke(this);
        
        writeStateObject.Show();
        if (_canvas == null) CreateCanvas();
        _canvas.Show();
    }

    public void HideUI() {
        _objectUIState = ObjectUIState.Idle;
        EventBus.Instance.OnVocabItemIdleState.Invoke(this);
        
        menuStateObject.Hide();
        writeStateObject.Hide(); // In case another VocabItem wants to be focused
    }

    public void DisableOutline() {
        if (_objectUIState == ObjectUIState.Menu) return;

        _outline.OutlineMode = Outline.Mode.OutlineHidden;
    }

    public void EnableSelectedOutline() {
        _outline.OutlineColor = _vocabularyState == VocabularyState.Learned ? Color.green : Color.red;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    public void EnableHoverOutline() {
        _outline.OutlineColor = Color.white;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
    }
    
    // ======================
    // Event listener methods
    // ======================

    void OnVocabItemFinishGuess.IHandler.OnEvent(VocabItem vocabItem, string guess) {
        StartCoroutine(DelayedHide());
        Wordle(guess);
    }

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string character) {
        StartCoroutine(DelayedCreate());
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
            DisplayToast("Correct!<br>Good job!");
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
        
        LogEvent($"User was close, their guess: {guess}");
        DisplayToast($"Try again!<br>Your guess: {guessUIText}");
    }
    
    // TODO: remove anything related to *clearing* the canvas, you may not need it
    private IEnumerator DelayedCreate() {
        DestroyCanvas();
        
        StartCoroutine(ClearCanvas());
        yield return new WaitForSecondsRealtime(0.1f);
        
        CreateCanvas();
    }

    private IEnumerator DelayedHide() {
        StartCoroutine(ClearCanvas());
        yield return new WaitForSecondsRealtime(0.3f);
        DestroyCanvas();
    }

    private void DestroyCanvas() {
        Destroy(_canvas.gameObject);
        _canvas = null;
        
        LogEvent("Canvas destroyed");
    }

    private void SetUpEraserCube() {
        Vector3 canvasSpawnPos = canvasSpawnPoint.position;
        _canvasEraserCube.transform.SetLocalPositionAndRotation(canvasSpawnPos + (Vector3.down * 10f), canvasSpawnPoint.rotation);
        _canvasEraserCube.SetActive(false);
    }

    private IEnumerator ClearCanvas() {
        var cubePosition = canvasSpawnPoint.localPosition;
        _canvasEraserCube.SetActive(true);

        yield return new WaitForSecondsRealtime(0.15f);
        LogEvent("Canvas clearing object appeared");
        
        // Move it up a bit
        _canvasEraserCube.transform.localPosition = new Vector3(
            cubePosition.x,
            cubePosition.y + 0.5f,
            cubePosition.z - 0.5f
            ); 
        
        // wait for it
        yield return new WaitForSecondsRealtime(0.15f);

        // and move it out of the way
        // the pen ink won't show if it's just disabled
        _canvasEraserCube.transform.Translate((Vector3.down * 10f), transform.parent);
        _canvasEraserCube.SetActive(false);
        LogEvent("Canvas clearing object disappeared");
    }

    private void CreateCanvas() {
        _canvas = Instantiate(_canvasObject, canvasSpawnPoint.position, canvasSpawnPoint.rotation, transform)
            .GetComponent<LetterCanvas>();
        
        LogEvent("New canvas created");
    }
    
    // TODO ends here

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}