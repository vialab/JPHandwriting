using UnityEngine;

/// <summary>
/// Base class for objects/scripts that are loaded from a file by instantiating a prefab.
/// </summary>
public class SerializableScript<Self> : MonoBehaviour
where Self : SerializableScript<Self> {
    /// <summary>
    /// A unique identifier used for (de)serializing this object to/from a prefab.
    /// </summary>
    public string Type;

    public virtual void Load(SerializedScript<Self> serializedScript) {
        JsonUtility.FromJsonOverwrite(serializedScript.Data, this);

        // also serialized in Data, but assigning here makes it simpler to manually write beatmaps
        Type = serializedScript.Type;
    }

    public virtual SerializedScript<Self> Save() => new(
        type: Type,
        data: JsonUtility.ToJson(this)
    );
}
