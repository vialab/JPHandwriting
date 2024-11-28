using System;
using UnityEditor.PackageManager;

public class LogMessage {
    public LogLevel level;
    public string message;

    public LogMessage(LogLevel l, string m) {
        level = l;
        message = m;
    }

    public string GetLevelName() {
        var levelName = Enum.GetName(typeof(LogLevel), level);
        
        return levelName == null ? "DEBUG" : levelName.ToUpper();
    }

    public override string ToString() {
        return message;
    }
}