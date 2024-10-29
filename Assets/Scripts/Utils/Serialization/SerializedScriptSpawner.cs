using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SerializedScriptSpawner<T> : MonoBehaviour
where T : SerializableScript<T> {
    /// <summary>
    /// New objects will be instantiated with this object as their parent.
    /// </summary>
    [SerializeField] protected Transform collector;

    [SerializeField] protected List<T> prefabs;

    [field: SerializeField] public string EditorSpawnType { get; protected set; }
    [field: SerializeField, TextArea] public string EditorSpawnData { get; protected set; }

    protected readonly Dictionary<string, T> lookup = new();

    protected virtual void Awake() {
        BuildLookup(lookup, prefabs);
    }

    protected void BuildLookup(Dictionary<string, T> lookup, List<T> prefabs) {
        lookup.Clear();

        foreach (var prefab in prefabs) {
            if (string.IsNullOrEmpty(prefab.Type)) {
                throw new ArgumentException("Prefab type is missing or empty: " + prefab);
            }

            lookup.Add(prefab.Type, prefab);
        }
    }

    public virtual List<T> Spawn(IEnumerable<SerializedScript<T>> serializedScripts) {
        return serializedScripts.Select(Spawn).ToList();
    }

    public virtual T Spawn(SerializedScript<T> serializedScript) {
        var obj = Instantiate(lookup[serializedScript.Type], collector);
        obj.Load(serializedScript);
        return obj;
    }
}
