using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Network
{
    [DisallowMultipleComponent]
    public class RSInternetConnectivityComponent : RSSingletonMonoBehaviour<RSInternetConnectivityComponent>
    {
        public class RSUnityPingEvent : UnityEvent<NetworkReachability, System.Net.NetworkInformation.IPStatus> { }
        public RSUnityPingEvent OnRSUnityPingComplete { get; private set; } = new RSUnityPingEvent();
        public UnityEvent OnRSUnityPingStart { get; private set; } = new UnityEvent();

        [SerializeField] private int _timeoutms = 2000;
        [SerializeField] private int _attemptsMax= 3;
        [SerializeField] private int _attemptsLefts = 0;

        public int AttemptsLefts
        {
            get{
                return _attemptsLefts;
            }
        }

        public int AttemptsMax
        {
            get
            {
                return _attemptsMax;
            }
        }

        private Ping _ping;
        private float _pingStartTime;

        public bool StartPingIP(string ipAddress, bool resetAttempts = true)
        {
            _ping = null;
            if (resetAttempts)
            {
                _attemptsLefts = _attemptsMax;
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                LogErrorInDebugMode("No IP address provided. Aborting");
                return false;
            }
            LogInDebugMode($"Start Ping IP: {ipAddress}");
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                LogInDebugMode($"internetReachability: NotReachable. Aborting.");
                return false;
            }
            _ping = new Ping(ipAddress);
            _pingStartTime = Time.time;
            OnRSUnityPingStart.Invoke();
            return true;
        }

        public bool StartPingHost(string hostName = "unity.com")
        {
            return StartPingIP(RSInternetConnectivity.GetHostIPV4Address(hostName));
        }

        public bool IsPinging()
        {
            return _ping != null && !_ping.isDone;
        }

        #region MonoBehaviour Functions

        public void Update()
        {
            if (_ping == null)
            {
                return;
            }
            if (_ping.isDone)
            {
                if (_ping.time >= 0)
                {
                    OnRSUnityPingComplete.Invoke(Application.internetReachability, System.Net.NetworkInformation.IPStatus.Success);
                    LogInDebugMode("Ping Success");
                    _ping = null;
                }
                else
                {
                    LogErrorInDebugMode("Ping Fail!");
                    _attemptsLefts--;
                    if(_attemptsLefts > 0)
                    {
                        LogErrorInDebugMode("Retrying!");
                        StartPingIP(_ping.ip, false);
                    }
                    else
                    {
                        OnRSUnityPingComplete.Invoke(Application.internetReachability, System.Net.NetworkInformation.IPStatus.Unknown);
                    }
                }
            }
            else if (Time.time - _pingStartTime > _timeoutms / 1000f)
            {
                OnRSUnityPingComplete.Invoke(Application.internetReachability, System.Net.NetworkInformation.IPStatus.TimedOut);
                LogErrorInDebugMode("Ping Timeout!");
                _ping = null;
            }
        }

        #endregion MonoBehaviour Functions

    }
}
