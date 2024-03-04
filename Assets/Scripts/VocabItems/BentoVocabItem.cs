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

public class BentoVocabItem : _BaseVocabItem {
    [SerializeField] private GameObject menuStateGameObject;
    [SerializeField] private GameObject writingStateGameObject;
    [SerializeField] private TextMeshProUGUI wordText;

    private ObjectState _objectState;
    private VocabState _vocabState;
    
    
    void Awake() {
        menuStateGameObject.gameObject.SetActive(false);
        writingStateGameObject.gameObject.SetActive(false);
        wordText.text = englishName;
    }

    private void Start() {
        // TODO: find a way to set vocab state from another object
        _objectState = ObjectState.Idle;
        _vocabState = VocabState.NotLearned;
    }

    public void ToggleWritingUI() {
        switch (_objectState) {
            case ObjectState.Menu: {
                menuStateGameObject.gameObject.SetActive(false);
                writingStateGameObject.gameObject.SetActive(true);
                _objectState = ObjectState.Writing;
                break;
            }
            case ObjectState.Writing: {
                menuStateGameObject.gameObject.SetActive(true);
                writingStateGameObject.gameObject.SetActive(false);
                _objectState = ObjectState.Menu;
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
               menuStateGameObject.gameObject.SetActive(true);
               _objectState = ObjectState.Menu;
               break;
            }
            case ObjectState.Menu: {
                menuStateGameObject.gameObject.SetActive(false);
                _objectState = ObjectState.Idle;
                break;
            }
            case ObjectState.Writing:
            default: {
                break;
            }
        }
    }
}
