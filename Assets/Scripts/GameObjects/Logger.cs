using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

public class Logger : EventSubscriber, OnLoggableEvent.IHandler, OnLetterExported.IHandler {
    /// <summary>
    /// The time interval, in seconds, between the last time the logger wrote to the log file and the next.
    /// </summary>
    [Rename("Log Interval (seconds)")]
    [Tooltip("The time interval between the last time the logger wrote to the log file and the next.")]
    [SerializeField] private float logIntervalTime = 3f;

    private readonly List<LogMessage> eventLog = new(); 
    
    private DateTime _startTime;

    [SerializeField] private string logFolder = "SessionLogs";
    [SerializeField] private string logFolderDebug = "Debug";
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
        
        LogFromLogger("Session started, logger initialized");
        
        InvokeRepeating(nameof(WriteToFile), logIntervalTime, logIntervalTime);
    }
    
    // OnDestroy because it'll be the very last thing that's done before closing
    private void OnDestroy() {
        WriteToFile();
    }

    private void WriteToFile() {
        if (eventLog.Count == 0) return;
        
        using var outFile = new StreamWriter(Path.Combine(logFolder, userID, logFileName), append: true);
        using var debugOutFile = new StreamWriter(Path.Combine(logFolder, logFolderDebug, userID, logFileName), append: true);
        
        debugOutFile.Write(string.Join("\n", eventLog)); // Write debug stuff like coordinates
        outFile.Write(string.Join("\n", eventLog.Where(x => x.level == LogLevel.Info).ToList())); // Write everything needed for researchers to see
        
        // End of lists for both, \n not added after last element of array
        debugOutFile.Write("\n"); 
        outFile.Write("\n"); 
        
        eventLog.Clear();
    }

    private void LogEvent(string text, LogLevel level) {
        var message = new LogMessage(level, text);
        Debug.Log($"{message.GetLevelName()}: {message.message}");
        eventLog.Add(message);
    }

    private void LogFromLogger(string text, LogLevel level = LogLevel.Info) {
        LogEvent($"[{name}] ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) {text}", level);
    }

    void OnLoggableEvent.IHandler.OnEvent(UnityEngine.Object obj, string text, LogLevel level) {
        var objName = obj.name;
        
        // So you don't see (Clone) in logs
        if (obj.GetComponent<VocabItem>() != null) {
            objName = obj.GetComponent<VocabItem>().Type;
        }
        
        LogEvent($"[{objName}] ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) {text}", level);
    }

    void OnLetterExported.IHandler.OnEvent(VocabItem vocabItem, byte[] image) {
        // save image
        File.WriteAllBytes(Path.Join(logFolder, userID, imageFileName), image);
        LogFromLogger($"Image {imageFileName} written");
    }
}
