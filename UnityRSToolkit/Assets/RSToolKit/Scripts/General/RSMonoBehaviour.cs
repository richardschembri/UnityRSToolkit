using UnityEngine;
using RSToolkit.Helpers;
namespace RSToolkit
{
    public class RSMonoBehaviour : MonoBehaviour
    {
        public bool DebugMode = false;
        public bool InitOnAwake = false;
        public bool Initialized { get; protected set; } = false;

        protected virtual void Init()
        {
            Initialized = true;
        }

        #region MonoBehaviour Functions
        protected virtual void Awake()
        {
            if (InitOnAwake)
            {
                Init();
            }
        }
        #endregion MonoBehaviour Functions

        public virtual string GetDebugTag()
        {
            return string.Empty;
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