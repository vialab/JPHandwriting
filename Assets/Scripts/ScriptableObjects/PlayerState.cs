using System.Collections.Generic;
using UnityEngine;

public class PlayerState : ScriptableObject {
    public string participantID;
    public List<VocabItem> learnedVocabulary;
    public List<VocabItem> shownVocabulary;
    public VocabItem currentVocabulary;
}
