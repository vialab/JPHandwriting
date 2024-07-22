using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum VocabState {
    NotLearned, Learned 
}

public enum ObjectState {
    Idle,
    Menu,
    Writing
}

public class _BaseVocabItem : MonoBehaviour {
   [SerializeField] private Transform markerSpawnPoint, drawBoardSpawnPoint;
   public string englishName, japaneseName, japaneseRomaji;
   public AudioClip pronunciationClip;
   
   [SerializeField] private GameObject menuStateGameObject;
   [SerializeField] private ItemDrawStateScript writingStateGameObject;
   public ItemDrawStateScript WritingStateGameObject => writingStateGameObject;
   
   [SerializeField] private TextMeshProUGUI wordText;

   [SerializeField] private GameObject toastGUI;
   [SerializeField] private TextMeshProUGUI toastText;

   private ObjectState _objectState;
   private VocabState _vocabState;
   private Outline _outline;
   
   void Awake() {
       menuStateGameObject.gameObject.SetActive(false);
       writingStateGameObject.gameObject.SetActive(false);
       toastGUI.SetActive(false);
       wordText.SetText(englishName);
       _outline = gameObject.AddComponent<Outline>();
   }

   private void Start() {
       // Set states
       _objectState = ObjectState.Idle;
       _vocabState = VocabState.NotLearned;
       
       // Set outline stuff 
       _outline.enabled = false;
       _outline.OutlineWidth = 5f;
       _outline.OutlineMode = Outline.Mode.OutlineVisible;
   }

   public void ToggleWritingUI() {
       switch (_objectState) {
           case ObjectState.Menu: {
               DisableMenuStateObject();
               EnableWritingStateObject();
               _outline.enabled = false;
               
               SetObjectState(ObjectState.Writing);
               break;
           }
           case ObjectState.Writing: {
               EnableMenuStateObject();
               DisableWritingStateObject();
               EnableSelectedOutline();
               
               SetObjectState(ObjectState.Menu);
               break;
           }
           case ObjectState.Idle:
           default: {
               break;
           }
       }
   }

   public void ToggleUI() {
       switch (_objectState) {
           case ObjectState.Idle: {
              EnableMenuStateObject();
              EnableSelectedOutline();
              SetObjectState(ObjectState.Menu);

              if (PredictionControllerScript.instance.FocusedVocabItem is not null) {
                  // untoggle the old focused item 
                  PredictionControllerScript.instance.FocusedVocabItem.ToggleUI();
              }
              
              PredictionControllerScript.instance.FocusedVocabItem = this;
              break;
           }
           case ObjectState.Menu: {
               DisableMenuStateObject();
               DisableOutline();
               SetObjectState(ObjectState.Idle);

               PredictionControllerScript.instance.FocusedVocabItem = null;
               break;
           }
           case ObjectState.Writing:
           default: {
               break;
           }
       }
   }

   public void EnableHoverOutline() {
       if (_objectState == ObjectState.Menu) {
           return;
       }
       
       _outline.OutlineColor = Color.white;
       _outline.enabled = true;
   }
   public void EnableSelectedOutline() {
       _outline.OutlineColor = _vocabState == VocabState.Learned ? Color.green : Color.red;
       _outline.enabled = true;
   }

   public void DisableOutline() {
       if (_objectState == ObjectState.Menu) {
           return;
       }
       
       _outline.enabled = false;
   }
   
   // a lot of helper functions
   private void EnableMenuStateObject() {
       ActivityLogger.Instance.LogEvent("In menu for item", this);
       menuStateGameObject.gameObject.SetActive(true);
   }
   
   private void DisableMenuStateObject() {
       menuStateGameObject.gameObject.SetActive(false);
   }

   private void EnableWritingStateObject() {
       ActivityLogger.Instance.LogEvent("Attempting to write item", this);
       writingStateGameObject.gameObject.SetActive(true);
   }

   private void DisableWritingStateObject() {
       writingStateGameObject.gameObject.SetActive(false);
   }
   
   // NOTE: maybe not private?
   private void SetObjectState(ObjectState state) {
       _objectState = state;
   }

   public void ToggleLearned(string compareTo) {
       _vocabState = (compareTo == japaneseName) ? VocabState.Learned : VocabState.NotLearned;
       ActivityLogger.Instance.LogEvent($"New state for word is {_vocabState}", this);
       
       // have toast pop up I think
       string messageToSend = (_vocabState == VocabState.Learned) ? "Correct!" : "Try again...";
       StartCoroutine(SummonToast(messageToSend));

       if (_objectState == ObjectState.Writing) {
           ToggleWritingUI();
       }
   }

   IEnumerator SummonToast(string message) {
       toastText.SetText(message);
       toastGUI.SetActive(true);

       yield return new WaitForSeconds(3);
       
       toastGUI.SetActive(false);
   }
}
