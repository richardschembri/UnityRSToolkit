using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Network{
    public class RSNetworkObject : RSShadow
    {
        public enum NetworkTypes
        {
            None,
            Owner,
            Peer
        }

        public class OnNetworkTypeChangedEvent : UnityEvent<NetworkTypes> {} 
        public static OnNetworkTypeChangedEvent OnNetworkTypeChanged {get; set;} = new OnNetworkTypeChangedEvent(); 

        private static NetworkTypes _networkType = NetworkTypes.None;
        public static NetworkTypes NetworkType
        {
            get
            {
                return _networkType;
            }
            set
            {
                if(_networkType != value){
                    _networkType = value;
                    OnNetworkTypeChanged.Invoke(_networkType);
                }
            }
        }

        public static bool IsNetworkPeer
        {
            get
            {
                return RSNetworkObject.NetworkType == RSNetworkObject.NetworkTypes.Peer;
            }
        }

        public static bool IsNetworkOwner
        {
            get
            {
                return RSNetworkObject.NetworkType == RSNetworkObject.NetworkTypes.Owner;
            }
        }

        #region MonoBehaviour Functions
        protected override void Awake()
        {
            OnNetworkTypeChanged.AddListener(OnNetworkTypeChanged_Listener);
            base.Awake();
        }

        #endregion MonoBehaviour Functions

        protected virtual void OnNetworkTypeChanged_Listener(NetworkTypes networkType){
            base.TrySetIsShadown(RSNetworkObject.IsNetworkPeer);
        }

        public override bool TrySetIsShadown(bool on)
        {
            return false;
        }

        public override bool IsShadow(){
            return RSNetworkObject.IsNetworkPeer;
        }
    }
}