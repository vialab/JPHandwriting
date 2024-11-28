using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// An interface that provides methods for spawning objects on top of other items.
/// Implementation from https://stackoverflow.com/q/30209053
/// </summary>
public interface IObjectMover {
    List<Transform> GetSpawnRows();

    List<Vector3> CreateSpawnPositions() {
        // spawnRows is a list of objects with objects inside it
        // we need the positions of objects inside it
        var spawnRows = GetSpawnRows();
        List<Vector3> spawns = new();

        foreach (var row in spawnRows) {
            foreach (Transform child in row.transform) {
                var originalPos = child.transform.position;
                
                var platformSize = child.gameObject.GetComponent<Renderer>().bounds.size.y;
                var platformTop = originalPos.y + platformSize / 2;
                
                spawns.Add(new Vector3(originalPos.x, platformTop, originalPos.z));
            }
        }

        return spawns;
    }

    VocabItem PlaceItem(VocabItem item, Vector3 position) {
        var objectSize = item.gameObject.GetComponentInChildren<Renderer>().bounds.size.y;
        var objectCentre = new Vector3(position.x, position.y + objectSize / 2, position.z);
        
        item.transform.position = objectCentre;
        item.transform.rotation = Quaternion.identity;

        return item;
    }
}