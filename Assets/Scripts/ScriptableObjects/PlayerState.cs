using System.Collections.Generic;
using UnityEngine;

public class PlayerState : ScriptableObject {
    public string participantID;
    public List<_BaseVocabItem> learnedVocabulary;
    public List<_BaseVocabItem> shownVocabulary;
    public _BaseVocabItem currentVocabulary;
}
