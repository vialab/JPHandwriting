using UnityEditor.PackageManager;
using UnityEngine;

public interface ILoggable {
    void LogEvent(string message, LogLevel level = LogLevel.Info);
}