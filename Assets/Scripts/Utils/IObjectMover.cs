using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public interface IObjectMover {
    List<Transform> GetSpawnRows();

    List<Vector3> CreateSpawnPositions() {
        // spawnRows is a list of objects with objects inside it
        // we need the positions of objects inside it
        var spawnRows = GetSpawnRows();
        List<Vector3> spawns = new();

        foreach (var row in spawnRows) {
            foreach (Transform child in row.transform) {
                spawns.Add(child.position);
            }
        }

        return spawns;
    }

    VocabItem PlaceItem(VocabItem item, Vector3 position, float offset) {
        item.transform.position = position + Vector3.up * offset;

        return item;
    }
}