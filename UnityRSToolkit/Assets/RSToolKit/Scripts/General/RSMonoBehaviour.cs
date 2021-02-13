using UnityEngine;
using RSToolkit.Helpers;
using UnityEngine.Events;

namespace RSToolkit
{
    public class RSMonoBehaviour : MonoBehaviour
    {
        public bool DebugMode = false;
        public bool InitOnAwake = false;
        public bool Initialized { get; protected set; } = false;


        public class RSMonoBehaviourEvent : UnityEvent<RSMonoBehaviour> {}
        public RSMonoBehaviourEvent OnAwake = new RSMonoBehaviourEvent();
        public RSMonoBehaviourEvent OnDestroyed = new RSMonoBehaviourEvent();

        public virtual bool Init(bool force = false)
        {
            if(Initialized && !force)
            {
                return false;
            }
            Initialized = true;
            return true;
        }

        public virtual void ResetValues(){}

        #region MonoBehaviour Functions
        protected virtual void Awake()
        {
            if (InitOnAwake)
            {
                Init();
            }

            OnAwake.Invoke(this);
        }
        protected virtual void OnDestroy()
        {
            OnDestroyed.Invoke(this);
        }
        #endregion MonoBehaviour Functions

        #region  Debug

        public virtual string GetDebugTag()
        {
            return $"{gameObject.name}";
        }

        protected void LogInDebugMode(string message, bool includeTimestamp = false)
        {
            DebugHelpers.LogInDebugMode(DebugMode, GetDebugTag(), message, includeTimestamp);
        }

        protected void LogErrorInDebugMode(string message, bool includeTimestamp = false)
        {
            DebugHelpers.LogErrorInDebugMode(DebugMode, GetDebugTag(), message, includeTimestamp);
        }

        #endregion Debug

    }
}