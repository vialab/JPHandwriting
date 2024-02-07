using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _BaseVocabItem : MonoBehaviour {
   [SerializeField] private Transform markerSpawnPoint, drawBoardSpawnPoint;
   public string englishName, japaneseName, japaneseRomaji;
}
