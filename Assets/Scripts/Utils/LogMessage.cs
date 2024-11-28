using System;
using ExtensionMethods;
using UnityEditor.PackageManager;

public class LogMessage {
    public LogLevel level;
    public string message;

    public LogMessage(LogLevel l, string m) {
        level = l;
        message = m;
    }

    public string GetLevelName() {
        var levelName = $"{Enum.GetName(typeof(LogLevel), level)?.ToUpper()}".Bold();

        switch (level) {
            case LogLevel.Info:
                levelName = levelName.WithColor("green");
                break;
            case LogLevel.Debug:
                levelName = levelName.WithColor("yellow");
                break;
            case LogLevel.Error:
                levelName = levelName.WithColor("red");
                break;
            case LogLevel.Silly:
            case LogLevel.Verbose:
            case LogLevel.Warn:
                levelName = levelName.WithColor("orange");
                break;
            default:
                levelName = "DEBUG".Bold().WithColor("yellow");
                break;
        }
        
        return levelName;
    }

    public override string ToString() {
        return message;
    }
}