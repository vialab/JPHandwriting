using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class VocabItemSpawner : SerializedScriptSpawner<VocabItem>, IObjectMover {
    public float offset = 0.0f;
    [ReadOnly] private List<Transform> spawnRows = new();
    [ReadOnly] private List<Vector3> spawns = new();

    protected override void Awake() {
        base.Awake();
    }

    public void SetOffset(float offset) {
        this.offset = offset;
    }

    public void SetSpawns(List<Transform> spawnRows) {
        this.spawnRows = spawnRows;
        spawns = ((IObjectMover)this).CreateSpawnPositions();
    }

    public override VocabItem Spawn(SerializedScript<VocabItem> serializedScript) {
        var vocabItem = base.Spawn(serializedScript);

        var index = RandomNumberGenerator.GetInt32(0, spawns.Count);
        var pos = spawns[index];

        spawns.RemoveAt(index);

        return ((IObjectMover)this).PlaceItem(vocabItem, pos, offset);
    }

    public List<Transform> GetSpawnRows() => spawnRows;
}
