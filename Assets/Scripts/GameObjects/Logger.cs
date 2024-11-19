using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Logger : EventSubscriber, OnLoggableEvent.IHandler, OnLetterExported.IHandler {
    /// <summary>
    /// The time interval, in seconds, between the last time the logger wrote to the log file and the next.
    /// </summary>
    [Rename("Log Interval (seconds)")]
    [Tooltip("The time interval between the last time the logger wrote to the log file and the next.")]
    [SerializeField] private float logIntervalTime = 3f;

    private readonly List<string> eventLog = new(); 
    private DateTime _startTime;

    [SerializeField] private string logFolder = "SessionLogs";
    private string userID;
    
    private string filenameBase => $"KikuKaku_{userID}";
    private string logFileName => $"{filenameBase}_{_startTime:yyyy-MM-dd_HH-mm-ss}.txt";
    
    private string imageFileName => $"{filenameBase}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";

    private void Awake() {
        _startTime = DateTime.Now;
        userID = Game.Instance.userID;
    }

    protected override void Start() {
        base.Start();
        
        LogEvent($"[{name}] ({_startTime:yyyy-MM-dd HH:mm:ss}) Session started, logger initialized");
        
        InvokeRepeating(nameof(WriteToFile), logIntervalTime, logIntervalTime);
    }
    
    // OnDestroy because it'll be the very last thing that's done before closing
    private void OnDestroy() {
        WriteToFile();
    }

    private void WriteToFile() {
        if (eventLog.Count == 0) return;
        
        using var outFile = new StreamWriter(Path.Join(logFolder, userID, logFileName), append: true);
        outFile.Write(string.Join("\n", eventLog));
        outFile.Write("\n"); // newline isn't added after last element of eventLog array 
        eventLog.Clear();
    }

    private void LogEvent(string text) {
        Debug.Log(text);
        eventLog.Add(text);
    }

    void OnLoggableEvent.IHandler.OnEvent(UnityEngine.Object obj, string text) {
        var objName = obj.name;
        
        // So you don't see (Clone) in logs
        if (obj.GetComponent<VocabItem>() != null) {
            objName = obj.GetComponent<VocabItem>().Type;
        }
        
        LogEvent($"[{objName}] ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) {text}");
    }

    void OnLetterExported.IHandler.OnEvent(VocabItem vocabItem, byte[] image) {
        // save image
        File.WriteAllBytes(Path.Join(logFolder, userID, imageFileName), image);
        LogEvent($"Image {imageFileName} written");
    }
}
