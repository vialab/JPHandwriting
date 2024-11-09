using System.IO;
using UnityEngine;

public static class JsonFile {
    public static T ReadFile<T>(string path) {
        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    public static void WriteFile(string path, object obj) {
        var json = JsonUtility.ToJson(obj);
        var directory = Path.GetDirectoryName(path);
        Directory.CreateDirectory(directory);
        File.WriteAllText(path, json);
    }

    public static T ReadAsset<T>(string path) {
        var asset = Resources.Load<TextAsset>(path);
        return JsonUtility.FromJson<T>(asset.text);
    }
}