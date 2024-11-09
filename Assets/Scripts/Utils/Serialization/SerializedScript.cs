using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSON-serialized representation of a SerializableScript instance.
/// </summary>
[Serializable]
public record SerializedScript<T>
where T : SerializableScript<T> {
    public string Type;
    public string Data;

    public SerializedScript(string type, string data) {
        Type = type;
        Data = data;
    }

    public T Instantiate(Dictionary<string, T> prefabs) {
        var instance = UnityEngine.Object.Instantiate(prefabs[Type]);
        instance.Load(this);
        return instance;
    }

    public T Instantiate(Dictionary<string, T> prefabs, Vector3 position) {
        var instance = UnityEngine.Object.Instantiate(prefabs[Type], position, Quaternion.identity);
        instance.Load(this);
        return instance;
    }
}
