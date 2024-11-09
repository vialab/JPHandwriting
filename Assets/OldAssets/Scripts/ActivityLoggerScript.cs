using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class ActivityLogger : MonoBehaviour {
    [SerializeField] private string sessionID = "001";
    private DateTime _initializationTime;
    public static ActivityLogger Instance;
    
    private string filename => $"KikuKaku_{sessionID}_{_initializationTime:yyyy-MM-dd_HH-mm-ss}";
    public string logFilename => $"{filename}.txt";
    public string imageFilename => $"KikuKaku_{sessionID}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";

    private void Awake() {
        // Get time of script initialization/game start
        _initializationTime = DateTime.Now;

        Instance ??= this;
    }

    private void Start() {
        LogToFile($"Logger initialized!", Instance);
    }

    public void LogEvent(string text, Object caller) {
        LogToFile(text, caller);
    }
    
    // Actual logging. Might probably be a coroutine or something else "async"
    private void LogToFile(string text, Object caller) {
        using var outFile = new StreamWriter(logFilename, true);
        
        DateTime logTime = DateTime.Now;
        outFile.WriteLine($"[{caller.name}] ({logTime:yyyy-MM-dd HH:mm:ss}) {text}");
        Debug.Log(text);
    }
}