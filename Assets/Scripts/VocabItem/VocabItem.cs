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
    [SerializeField] protected string englishName;
    
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
    // State objects
    // =============
    
    /// <summary>
    /// The object that controls the UI in Menu state. 
    /// </summary>
    [SerializeField] protected Menu menuStateObject;
    
    /// <summary>
    /// The object that controls the UI in Writing state.
    /// </summary>
    [SerializeField] protected VocabItemWrite writeStateObject;
    

    // ==========
    // UI objects
    // ==========
    
    /// <summary>
    /// The object that controls the "toast" notification.
    /// </summary>
    [SerializeField] protected Toast toastObject;

    /// <summary>
    /// The object that shows the letter to be traced.
    /// </summary>
    [SerializeField] protected Tracing tracingUIObject;
    
    /// <summary>
    /// The coloured outline of the object.
    /// Green if the user has learned it, red otherwise.
    /// </summary>
    protected Outline _outline;
    

    // ==========================
    // WriteState-specific things
    // ==========================
    
    /// <summary>
    /// Whether to show the character to trace over or not.
    /// </summary>
    [Rename("Toggle Tracing")]
    [SerializeField] protected bool enableTracing = false;

    /// <summary>
    /// How long to wait for the canvas clearing cube to clear the canvas.
    /// </summary>
    [Rename("Eraser Cube Wait Time")]
    [SerializeField] protected float waitTimeSeconds = 0.15f;
 
    [SerializeField] protected LetterCanvas _canvas;
    [SerializeField] protected GameObject _canvasEraserCube;

    public string LetterAtCurrentPosition => JPName.ToArray()[writeStateObject.CharPosition].ToString();
    public int CurrentPosition => writeStateObject.CharPosition;


    // =====================
    // Other item properties
    // =====================
    // TODO: load from user config
   
    protected VocabularyState _vocabularyState = VocabularyState.NotLearned;
    protected ObjectUIState _objectUIState = ObjectUIState.Idle;


    // ===========================
    // Unity MonoBehaviour methods
    // ===========================

    protected void Awake() {
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
            
            StartCoroutine(ClearAndHideCanvas());
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

    private void HideUI() {
        _objectUIState = ObjectUIState.Idle;
        EventBus.Instance.OnVocabItemIdleState.Invoke(this);
        
        menuStateObject.Hide();
        writeStateObject.Hide(); // In case another VocabItem wants to be focused
    }

    public void ToggleUI() {
        if (_objectUIState == ObjectUIState.Idle) ShowMenuState();
        else HideUI();
    }


    // =======================
    // Outline display methods 
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

    
    // ======================
    // Event listener methods
    // ======================

    void OnVocabItemFinishGuess.IHandler.OnEvent(VocabItem vocabItem, string guess) {
        StartCoroutine(ClearAndHideCanvas());
        Wordle(guess);
    }

    void OnLetterPredicted.IHandler.OnEvent(VocabItem vocabItem, string character, int position) {
        StartCoroutine(DelayedUpdateCanvas(character, position));
    }
    

    // ==================
    // WriteState methods 
    // ==================

    private void SetUpEraserCube() {
        var canvasTransform = _canvas.transform;
        
        _canvasEraserCube.transform.SetPositionAndRotation(
            canvasTransform.position + (Vector3.down * 10f), 
            canvasTransform.rotation
        );
        
        _canvasEraserCube.SetActive(false);
    }

    private void SetUpTracingUI() {
        Transform tracingTransform;
        Transform canvasTransform = _canvas.transform;
        (tracingTransform = tracingUIObject.transform).SetPositionAndRotation(
            canvasTransform.position + (Vector3.up * 0.0001f), 
            canvasTransform.rotation
        );

        var angles = tracingTransform.localEulerAngles;
        angles.y = 0f;
        tracingTransform.localEulerAngles = angles;
        
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
            EventBus.Instance.OnVocabItemLearned.Invoke(this);
            
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
        var cubePosition = _canvas.transform.position;
        _canvasEraserCube.SetActive(true);

        LogEvent("Canvas clearing object appeared");
        
        // Move the cube up a bit
        _canvasEraserCube.transform.position = new Vector3(
            cubePosition.x,
            cubePosition.y + 0.005f,
            cubePosition.z - 0.005f
            ); 
        
        Debug.Log($"Eraser is {_canvasEraserCube.transform.position}", _canvasEraserCube);
        Debug.Log($"Canvas is {cubePosition}", _canvas);
        
        // wait for it
        yield return new WaitForSecondsRealtime(waitTimeSeconds);

        // and move it out of the way
        // the pen ink won't show if it's just disabled
        _canvasEraserCube.transform.Translate(Vector3.down * 10f, transform.parent);
        _canvasEraserCube.SetActive(false);
        LogEvent("Canvas clearing object disappeared");
    }

    /// <summary>
    /// Clears the canvas, and then hides it after.
    /// </summary>
    private IEnumerator ClearAndHideCanvas() {
        yield return StartCoroutine(ClearCanvas());
        _canvas.Hide();
    }
    
    /// <summary>
    /// Clears the canvas, and optionally gives the user the next character to trace.
    /// </summary>
    private IEnumerator DelayedUpdateCanvas(string writtenChar, int position) {
        yield return StartCoroutine(ClearCanvas());
        
        // if this works i'll cry
        
        if (enableTracing) {
            Debug.Log("Tracing is enabled");
            if (writtenChar == JPName[position].ToString()) {
                EventBus.Instance.OnLetterCompared.Invoke(this, writtenChar, true);

                if (writeStateObject.CharPosition < JPName.Length) {
                    var nextChar = JPName[writeStateObject.CharPosition].ToString();

                    if (Hiragana.IsSmall(nextChar))
                        nextChar = Hiragana.TryToggleSmall(nextChar);
                    
                    LogEvent($"Setting next character to trace to {nextChar}");
                    tracingUIObject.SetCharacter(nextChar);
                } else {
                    tracingUIObject.SetCharacter("");
                }
            } else {
                EventBus.Instance.OnLetterCompared.Invoke(this, writtenChar, false);
            }
        } else {
            EventBus.Instance.OnLetterCompared.Invoke(this, writtenChar, true);
        }
    }


    // ==========
    // Log method 
    // ==========

    public void LogEvent(string message) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message);
    }
}