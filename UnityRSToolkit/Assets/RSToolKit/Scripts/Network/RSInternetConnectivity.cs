using UnityEngine;
using System.Net;
using System;
using SystemNetworkInformation = System.Net.NetworkInformation;

// Taken from https://forum.unity.com/threads/how-to-check-internet-connection-in-an-app.384541/
namespace RSToolkit.Network
{
    public static class RSInternetConnectivity
    {
        public static uint TimeOutms = 10000;

        private static SystemNetworkInformation.PingOptions _pingOptions = new SystemNetworkInformation.PingOptions(64, true);

        private static byte[] _pingBuffer = new byte[32];
        public static string GetHostIPV4Address(string hostName)
        {
            IPHostEntry host;

            try
            {
                host = Dns.GetHostEntry(hostName);

                for (int i = 0; i < host.AddressList.Length; i++)
                {
                    if (host.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) 
                    {
                        return host.AddressList[i].ToString();
                    }
                }

                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return string.Empty;
        }

        public static bool PingIP(out SystemNetworkInformation.IPStatus status, string ipAddress)
        {
            status = SystemNetworkInformation.IPStatus.Unknown;

            try
            {
                var ping = new SystemNetworkInformation.Ping();
                var reply = ping.Send(ipAddress, (int)TimeOutms, _pingBuffer, _pingOptions);
                status = reply.Status;

                if (reply.Status == SystemNetworkInformation.IPStatus.Success)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            finally { }
        }
        public static bool PingIP(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }
            SystemNetworkInformation.IPStatus status;
            return PingIP(out status, ipAddress);
        }

        public static bool PingHost(out SystemNetworkInformation.IPStatus status, string hostName = "unity.com")
        {
            return PingIP(out status, GetHostIPV4Address(hostName)); 
        }

        public static bool PingHost(string hostName = "unity.com")
        {
            SystemNetworkInformation.IPStatus status;
            return PingIP(out status, GetHostIPV4Address(hostName)); 
        }

        public static bool HasInternetConnectivity(string hostName = "unity.com")
        {
            return Application.internetReachability != NetworkReachability.NotReachable
                    && PingHost(hostName);
        }
    }
}
