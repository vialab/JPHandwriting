using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class Logger : EventSubscriber, OnLoggableEvent.IHandler, OnLetterExported.IHandler {
    /// <summary>
    /// The ID of the user. 
    /// Used to organize logs and related files made during a session.
    /// </summary>
    [Tooltip("The ID of the user. Used to organize logs and related files made during a session.")]
    [SerializeField] private string userID = "test";

    /// <summary>
    /// The time interval, in seconds, between the last time the logger wrote to the log file and the next.
    /// </summary>
    [Rename("Log Interval (seconds)")]
    [Tooltip("The time interval between the last time the logger wrote to the log file and the next.")]
    [SerializeField] private float logIntervalTime = 3f;

    private List<string> eventLog = new List<string>();
    private DateTime _startTime;

    [SerializeField] private string logFolder = "SessionLogs";
    private string filenameBase => $"KikuKaku_{userID}";
    private string logFileName => $"{filenameBase}_{_startTime:yyyy-MM-dd_HH-mm-ss}.txt";
    
    // TODO: https://youtu.be/DfcWOPpmw14
    private string imageFileName => $"{filenameBase}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";

    private void Awake() {
        _startTime = DateTime.Now;
    }

    protected override void Start() {
        base.Start();
        LogEvent($"[{name}] ({_startTime:yyyy-MM-dd HH:mm:ss}) Session started, logger initialized");
        
        InvokeRepeating(nameof(WriteToFile), logIntervalTime, logIntervalTime);
    }
    
    // OnApplicationQuit makes the most sense if you're reading it and have no idea what everything does
    // but! OnDisable executes after that and OnDestroy does way after so
    private void OnDestroy() {
        WriteToFile();
    }

    private void WriteToFile() {
        if (eventLog.Count == 0) return;
        
        using var outFile = new StreamWriter(Path.Join(logFolder, logFileName), append: true);
        outFile.Write(string.Join("\n", eventLog));
        eventLog.Clear();
    }

    private void LogEvent(string text) {
        eventLog.Add(text);
    }

    void OnLoggableEvent.IHandler.OnEvent(UnityEngine.Object obj, string text) {
        LogEvent($"[{obj.name}] ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) {text}");
    }

    void OnLetterExported.IHandler.OnEvent(VocabItem vocabItem, byte[] image) {
        // save image
        File.WriteAllBytes(Path.Join(logFolder, imageFileName), image);
    }
}
