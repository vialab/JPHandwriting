using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : EventSubscriber, OnLoggableEvent.IHandler {
    /// <summary>
    /// The ID of the user. 
    /// Used to organize logs and related files made during a session.
    /// </summary>
    [SerializeField] private string userID = "test";

    /// <summary>
    /// The time interval, in seconds(?), between the last time the logger wrote to the log file and the next.
    /// </summary>
    [SerializeField] private float logIntervalTime = 3f;

    private List<string> eventLog;
    private DateTime _startTime;

    private readonly string _logFolder = "SessionLogs";
    private string _filenameBase => $"KikuKaku_{userID}";
    private string _logFileName => $"{_filenameBase}_{_startTime:yyyy-MM-dd_HH-mm-ss}.txt";
    
    // TODO: https://youtu.be/DfcWOPpmw14
    private string _imageFileName => $"{_filenameBase}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";

    private void Awake() {
        _startTime = DateTime.Now;
    }

    protected override void Start() {
        base.Start();
        LogEvent($"[{name}] ({_startTime:yyyy-MM-dd HH:mm:ss}) Session started, logger initialized");
        
        InvokeRepeating(nameof(WriteToFile), logIntervalTime, logIntervalTime);
    }
    
    // OnApplicationQuit makes the most sense if you're reading it and have no idea what everything does
    // but OnDisable executes after that and OnDestroy does way after so
    private void OnDestroy() {
        WriteToFile();
    }

    private void WriteToFile() {
        if (eventLog.Count == 0) return;
        
        using var outFile = new StreamWriter(Path.Join(_logFolder, _logFileName), append: true);
        outFile.Write(string.Join("\n", eventLog));
        eventLog.Clear();
    }

    private void LogEvent(string text) {
        eventLog.Add(text);
    }

    void OnLoggableEvent.IHandler.OnEvent(UnityEngine.Object obj, string text) {
        LogEvent($"[{obj.name}] ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) {text}");
    }
}
