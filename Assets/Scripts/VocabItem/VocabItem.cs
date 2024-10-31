using System;
using TMPro;
using UnityEngine;

public class VocabItem : MonoBehaviour {
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
    private Outline _outline;
    
    // ===============
    // UI text objects
    // ===============
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private TextMeshProUGUI wordText;
    
    // =====================
    // Other item properties
    // =====================
    private VocabularyState _vocabularyState;
    private ObjectUIState _objectUIState;

    // ===========================
    // Unity MonoBehaviour methods
    // ===========================
    private void Awake() {
        throw new NotImplementedException();
    }

    private void Start() {
        throw new NotImplementedException();
    }
    
    // ==================
    // UI display methods
    // ==================
    
}