using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit
{
    [DisallowMultipleComponent]
    public class RSShadow : RSMonoBehaviour
    {
        public class RSShadowEvent : UnityEvent<bool>{}
        public RSShadowEvent OnRSShadowChanged = new RSShadowEvent();
        [SerializeField]
        private bool _isShadow = false;

        public virtual bool TrySetIsShadown(bool on){
            if(_isShadow != on){
                _isShadow = on;
                OnRSShadowChanged.Invoke(_isShadow);
                return true;
            }
            return false;
        }

        public virtual bool IsShadow(){
            return _isShadow;
        }
    }
}
