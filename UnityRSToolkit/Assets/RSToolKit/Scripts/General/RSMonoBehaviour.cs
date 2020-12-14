using UnityEngine;
using RSToolkit.Helpers;
namespace RSToolkit
{
    public class RSMonoBehaviour : MonoBehaviour
    {
        public bool DebugMode = false;
        
        public virtual string GetDebugTag()
        {
            throw new System.Exception("GetDebugTag not implemented");
        }

        protected void LogInDebugMode(string message, bool includeTimestamp = false)
        {
            DebugHelpers.LogInDebugMode(DebugMode, GetDebugTag(), message, includeTimestamp);
        }

        protected void LogErrorInDebugMode(string message, bool includeTimestamp = false)
        {
            DebugHelpers.LogErrorInDebugMode(DebugMode, GetDebugTag(), message, includeTimestamp);
        }

    }
}