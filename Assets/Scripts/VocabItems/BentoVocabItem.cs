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
    Listening,
    Writing
}

public class BentoVocabItem : _BaseVocabItem {
    [SerializeField] private GameObject _UIObject;
    [SerializeField] private TextMeshProUGUI _wordText;

    private ObjectState _objectState;
    void Awake()
    {
        _UIObject.gameObject.SetActive(false);
        _objectState = ObjectState.Idle;
        _wordText.text = englishName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleUI() {
        switch (_objectState) {
            case ObjectState.Idle: {
               _UIObject.gameObject.SetActive(true);
               _objectState = ObjectState.Menu;
               break;
            }
            case ObjectState.Menu: {
                _UIObject.gameObject.SetActive(false);
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
