using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum VocabState {
    NotLearned, Learned 
}

public enum ObjectState {
    Idle,
    Menu,
    Listening,
    Writing
}

public class BentoVocabItem : _BaseVocabItem {
    [SerializeField] private GameObject menuUIObject;
    [SerializeField] private TextMeshProUGUI wordText;

    private ObjectState _objectState;
    private VocabState _vocabState;
    void Awake()
    {
        menuUIObject.gameObject.SetActive(false);
        _objectState = ObjectState.Idle;
        _vocabState = VocabState.NotLearned;
        wordText.text = englishName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleWriting() {
        switch (_objectState) {
            case ObjectState.Menu: {
                menuUIObject.gameObject.SetActive(false);
                _objectState = ObjectState.Writing;
                break;
            }
            case ObjectState.Writing: {
                menuUIObject.gameObject.SetActive(true);
                _objectState = ObjectState.Menu;
                break;
            }
        }
    }

    public void ToggleUI() {
        switch (_objectState) {
            case ObjectState.Idle: {
               menuUIObject.gameObject.SetActive(true);
               _objectState = ObjectState.Menu;
               break;
            }
            case ObjectState.Menu: {
                menuUIObject.gameObject.SetActive(false);
                _objectState = ObjectState.Idle;
                break;
            }
            case ObjectState.Listening:
            case ObjectState.Writing:
            default: {
                break;
            }
        }
    }
}
