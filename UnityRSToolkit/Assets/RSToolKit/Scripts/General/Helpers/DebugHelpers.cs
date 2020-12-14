using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.Helpers
{
    public static class DebugHelpers
    {
        public static string GetLogText(string tag, object message, bool includeTimestamp = false)
        {
            return $"[{tag}]: {message}";
        }

        public static void LogInDebugMode(bool debugMode, string tag, object message, bool includeTimestamp = false)
        {
            if (!debugMode)
            {
                return;
            }

            Log(tag, message, includeTimestamp);
        }

        public static void LogErrorInDebugMode(bool debugMode, string tag, object message, bool includeTimestamp = false)
        {
            if (!debugMode)
            {
                return;
            }

            LogError(tag, message, includeTimestamp);
        }

        public static void LogErrorInDebugMode(bool debugMode, string tag, object message, Exception ex, bool includeTimestamp = false)
        {
            if (!debugMode)
            {
                return;
            }

            LogErrorInDebugMode(debugMode, tag, $"{message} : \n {ex.Message} \n {ex.StackTrace}", includeTimestamp);
        }

        public static void Log(string tag, object message, bool includeTimestamp = false)
        {
            Debug.Log(GetLogText(tag, message, includeTimestamp));
        }

        public static void LogError(string tag, object message, bool includeTimestamp = false)
        {
            Debug.LogError(GetLogText(tag, message, includeTimestamp));
        }
    }
}