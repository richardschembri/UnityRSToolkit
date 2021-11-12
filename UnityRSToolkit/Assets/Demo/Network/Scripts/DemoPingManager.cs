using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit;
using RSToolkit.Network;
using System;

namespace Demo
{
    public class DemoPingManager : RSMonoBehaviour
    {

        [SerializeField] private Toggle _togglePinging;
        [SerializeField] private Toggle _toggleResult;
        [SerializeField] private Text _pingTimestamp;
        [SerializeField] private Slider _sliderAttempts;
        [SerializeField] private Text _sliderAttemptsLabel;
        [SerializeField] private Button _buttonPing;

        public void ButtonPingOnClick_Listener()
        {
            _toggleResult.isOn = false;
            RSInternetConnectivityComponent.Instance.StartPingHost();
            _pingTimestamp.text = DateTime.Now.ToLongTimeString();
            _buttonPing.interactable = false;
        }

        private void SetAttemptControls()
        {
            _sliderAttempts.maxValue = RSInternetConnectivityComponent.Instance.AttemptsMax;
            _sliderAttempts.value = RSInternetConnectivityComponent.Instance.AttemptsLefts;
            _sliderAttemptsLabel.text = RSInternetConnectivityComponent.Instance.AttemptsLefts.ToString();
        }

        #region Events
        private void OnRSUnityPingComplete_Listener(NetworkReachability networkReachability, System.Net.NetworkInformation.IPStatus ipStatus)
        {
            _toggleResult.isOn = ipStatus == System.Net.NetworkInformation.IPStatus.Success; 

            _buttonPing.interactable = true;
        }

        private void OnRSUnityPingStart_Listener()
        {
            SetAttemptControls();
        }
        #endregion Events

        #region MonoBehaviour Functions
        void Start()
        {
            SetAttemptControls();
            
            RSInternetConnectivityComponent.Instance.OnRSUnityPingComplete.AddListener(OnRSUnityPingComplete_Listener);
            RSInternetConnectivityComponent.Instance.OnRSUnityPingStart.AddListener(OnRSUnityPingStart_Listener);
        }
        void Update()
        {
            _togglePinging.isOn = RSInternetConnectivityComponent.Instance.IsPinging();
        }
        #endregion MonoBehaviour Functions
    }
}
