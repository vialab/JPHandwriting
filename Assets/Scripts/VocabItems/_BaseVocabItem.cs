using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// shut up Rider this will be used eventually
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

   private ObjectState _objectState;
   private VocabState _vocabState;
   
   void Awake() {
       menuStateGameObject.gameObject.SetActive(false);
       writingStateGameObject.gameObject.SetActive(false);
       wordText.SetText(englishName);
   }

   private void Start() {
       // TODO: find a way to set vocab state from another object
       _objectState = ObjectState.Idle;
       _vocabState = VocabState.NotLearned;
   }

   public void ToggleWritingUI() {
       switch (_objectState) {
           case ObjectState.Menu: {
               DisableMenuStateObject();
               EnableWritingStateObject();
               
               SetObjectState(ObjectState.Writing);
               break;
           }
           case ObjectState.Writing: {
               EnableMenuStateObject();
               DisableWritingStateObject();
               
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
   
   // a lot of helper functions
   private void EnableMenuStateObject() {
       menuStateGameObject.gameObject.SetActive(true);
   }
   
   private void DisableMenuStateObject() {
       menuStateGameObject.gameObject.SetActive(false);
   }

   private void EnableWritingStateObject() {
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
       _vocabState = compareTo == japaneseName ? VocabState.Learned : VocabState.NotLearned;
   }
}
