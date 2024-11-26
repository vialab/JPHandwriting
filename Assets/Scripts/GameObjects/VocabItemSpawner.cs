using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class VocabItemSpawner : SerializedScriptSpawner<VocabItem> {
    [SerializeField] private List<Transform> spawnPositions = new();
    
    public override VocabItem Spawn(SerializedScript<VocabItem> serializedScript) {
        var vocabItem = base.Spawn(serializedScript);

        var pos = RandomNumberGenerator.GetInt32(0, spawnPositions.Count);

        var position = spawnPositions[pos];
        spawnPositions.RemoveAt(pos);

        vocabItem.transform.position = position.position;
        
        return vocabItem;
    }
}
