using System.Collections;
using System.Linq;
using UnityEngine;
using ExtensionMethods;
using System;

public class VocabItem : EventSubscriber, ILoggable, OnVocabItemFinishGuess.IHandler, OnLetterPredicted.IHandler {
    // =====================
    // Vocab item properties
    // =====================
    // TODO: set this from JSON
    
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
    /// The object that shows the letter to be traced.
    /// </summary>
    [SerializeField] private Tracing tracingUIObject;
    
    /// <summary>
    /// The coloured outline of the object.
    /// Green if the user has learned it, red otherwise.
    /// </summary>
    private Outline _outline;
    

    // ==========================
    // WriteState-specific things
    // ==========================
    
    /// <summary>
    /// Where the canvas should spawn when the item is in Write state.
    /// </summary>
    [SerializeField] private Transform canvasSpawnPoint;

    /// <summary>
    /// Whether to show the character to trace over or not.
    /// </summary>
    [Rename("Toggle Tracing")]
    [SerializeField] private bool enableTracing;

    /// <summary>
    /// How long to wait for the canvas clearing cube to clear the canvas.
    /// </summary>
    [Rename("Eraser Cube Wait Time")]
    [SerializeField] private float waitTimeSeconds = 0.15f;
 
    [SerializeField] private LetterCanvas _canvas;
    [SerializeField] private GameObject _canvasEraserCube;

    public string LetterAtCurrentPosition => JPName.ToArray()[writeStateObject.CharPosition].ToString();


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
        _outline = gameObject.AddComponent<Outline>();
    }

    protected override void Start() {
        base.Start();
        
        // Menu state setup
        menuStateObject.SetClip(pronunciationClip);
        menuStateObject.SetUIText(englishName);
        menuStateObject.Hide();
        
        // Hiding everything else
        writeStateObject.Hide();
        _canvas.Hide();
        toastObject.Hide();
        
        // Other setup
        SetUpEraserCube();
        SetUpTracingUI();
        SetOutline();
    }
    

    // ==================
    // UI display methods
    // ==================
    
    private void DisplayToast(string message) {
        StartCoroutine(toastObject.ShowToast(message));
    }

    public void ShowMenuState() {
        // disable everything from write state
        if (_objectUIState == ObjectUIState.Writing) {
            writeStateObject.Hide();

            if (enableTracing) {
                tracingUIObject.Hide();
            }
            
            StartCoroutine(ClearCanvas());
        }
        
        // change state
        _objectUIState = ObjectUIState.Menu;
        EventBus.Instance.OnVocabItemMenuState.Invoke(this);
        
        // show menu
        EnableSelectedOutline();
        menuStateObject.Show();
    }

    public void ShowWriteState() {
        // disable everything from menu state
        if (_objectUIState == ObjectUIState.Menu) {
            menuStateObject.Hide(); // probably redundant check
            DisableOutline();
        }
        
        // change state
        _objectUIState = ObjectUIState.Writing;
        EventBus.Instance.OnVocabItemWriteState.Invoke(this);
        
        // show write state UI
        writeStateObject.Show();
        
        // if tracing is on, show the character to trace
        if (enableTracing) {
            tracingUIObject.SetCharacter(JPName[writeStateObject.CharPosition].ToString());
            tracingUIObject.Show();
        }
        
        // show canvas
        _canvas.Show();
    }

    public void HideUI() {
        _objectUIState = ObjectUIState.Idle;
        EventBus.Instance.OnVocabItemIdleState.Invoke(this);
        
        menuStateObject.Hide();
        writeStateObject.Hide(); // In case another VocabItem wants to be focused
    }


    // =======================
    // Outline display methods 
    // =======================

    private void SetOutline() {
        _outline.enabled = false;
        _outline.OutlineWidth = 5f;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
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
        StartCoroutine(ClearCanvasAndDoSomething(_canvas.Hide));
        Wordle(guess);
    }

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string character) {
        StartCoroutine(ClearCanvas());
        StartCoroutine(DelayedUpdateTracing());
    }
    

    // ==================
    // WriteState methods 
    // ==================

    private void SetUpEraserCube() {
        Vector3 canvasSpawnPos = canvasSpawnPoint.position;
        _canvasEraserCube.transform.SetLocalPositionAndRotation(
            canvasSpawnPos + (Vector3.down * 10f), 
            canvasSpawnPoint.rotation
        );
        _canvasEraserCube.SetActive(false);
    }

    private void SetUpTracingUI() {
        tracingUIObject.transform.SetLocalPositionAndRotation(
            canvasSpawnPoint.position + (Vector3.up * 0.001f), 
            canvasSpawnPoint.rotation
        );
        tracingUIObject.Hide();
    }
    
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
                // Correct character, correct position
                guessUIText += currentChar.WithColor("green");
            } else if (japaneseName.Contains(Hiragana.TryToggleSmall(currentChar))) {
                // Correct character, wrong position
                // or user submitted small when they meant large, or vice versa
                guessUIText += currentChar.WithColor("yellow");
            } else {
                // Wrong character, wrong position
                guessUIText += currentChar;
            }
        }
        
        LogEvent($"User was close, their guess: {guess}");
        DisplayToast($"Try again!<br>Your guess: {guessUIText}");
    }
    
    /// <summary>
    /// "Clears" the canvas.
    /// Actually, spawns a big white cube for waitTimeSeconds to overwrite the contents of the canvas with pure white,
    /// and then despawns and moves the cube out of the way.
    /// </summary>
    private IEnumerator ClearCanvas() {
        var cubePosition = canvasSpawnPoint.localPosition;
        _canvasEraserCube.SetActive(true);

        yield return new WaitForSecondsRealtime(waitTimeSeconds);
        LogEvent("Canvas clearing object appeared");
        
        // Move the cube up a bit
        _canvasEraserCube.transform.localPosition = new Vector3(
            cubePosition.x,
            cubePosition.y + 0.5f,
            cubePosition.z - 0.5f
            ); 
        
        // wait for it
        yield return new WaitForSecondsRealtime(waitTimeSeconds);

        // and move it out of the way
        // the pen ink won't show if it's just disabled
        _canvasEraserCube.transform.Translate(Vector3.down * 10f, transform.parent);
        _canvasEraserCube.SetActive(false);
        LogEvent("Canvas clearing object disappeared");
    }

    /// <summary>
    /// Clears the canvas, and then allows another action to happen after that.
    /// </summary>
    /// <param name="DoSomething">The action to execute after ClearCanvas().</param>
    private IEnumerator ClearCanvasAndDoSomething(Action DoSomething) {
        yield return StartCoroutine(ClearCanvas());
        DoSomething();
    }

    private IEnumerator DelayedUpdateTracing() {
        yield return new WaitForSecondsRealtime(0.2f);
        
        // TODO: handle case where the traced letter is incorrect
        // TODO: maybe user wants to re-write a character to get practice with it?
        if (enableTracing) { 
            if (writeStateObject.CharPosition < JPName.Length)
                tracingUIObject.SetCharacter(JPName[writeStateObject.CharPosition].ToString());
            else 
                tracingUIObject.SetCharacter("");
        }
    }


    // ==========
    // Log method 
    // ==========

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}