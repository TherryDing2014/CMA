﻿using NativeWifi;
using System;
using System.Collections.Generic;
using System.Text;

namespace WifiExample
{
    public class APManager
    {
        /// <summary>
        /// Converts a 802.11 SSID to a string.
        /// </summary>
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.UTF8.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        //获取无线AP的MAC地址  
        static string ApMac(byte[] macAddr)
        {
            string tMac = "";
            for (int i = 0; i < macAddr.Length; i++)
            {
                tMac += macAddr[i].ToString("x2").PadLeft(2, '0').ToUpper();
            }
            return tMac;
        }

        // 字符串转Hex
        static string StringToHex(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.Default.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(Convert.ToString(byStr[i], 16));
            }
            return (sb.ToString().ToUpper());
        }

        public static string ConnectToSSID(string id, string key)
        {
            try
            {
                WlanClient client = new WlanClient();
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected &&
                        wlanIface.CurrentConnection.isState == Wlan.WlanInterfaceState.Connected)
                        return "网络已连接!";
                    else
                    {
                        Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                        foreach (Wlan.WlanAvailableNetwork network in networks)
                        {
                            string ssid = GetStringForSSID(network.dot11Ssid);
                            if (ssid == id)
                            {
                                String auth = string.Empty;
                                String cipher = string.Empty;
                                bool isNoKey = false;
                                String keytype = string.Empty;
                                switch (network.dot11DefaultAuthAlgorithm.ToString())
                                {
                                    case "IEEE80211_Open":
                                        auth = "open"; break;
                                    case "RSNA":
                                        auth = "WPA2PSK"; break;
                                    case "RSNA_PSK":
                                        auth = "WPA2PSK"; break;
                                    case "WPA":
                                        auth = "WPAPSK"; break;
                                    case "WPA_None":
                                        auth = "WPAPSK"; break;
                                    case "WPA_PSK":
                                        auth = "WPAPSK"; break;
                                }
                                switch (network.dot11DefaultCipherAlgorithm.ToString())
                                {
                                    case "CCMP":
                                        cipher = "AES";
                                        keytype = "passPhrase";
                                        break;
                                    case "TKIP":
                                        cipher = "TKIP";
                                        keytype = "passPhrase";
                                        break;
                                    case "None":
                                        cipher = "none"; keytype = "";
                                        isNoKey = true;
                                        break;
                                    case "WWEP":
                                        cipher = "WEP";
                                        keytype = "networkKey";
                                        break;
                                    case "WEP40":
                                        cipher = "WEP";
                                        keytype = "networkKey";
                                        break;
                                    case "WEP104":
                                        cipher = "WEP";
                                        keytype = "networkKey";
                                        break;
                                }

                                if (isNoKey && !string.IsNullOrEmpty(key))
                                {
                                    return "无法连接网络！";
                                }
                                else if (!isNoKey && string.IsNullOrEmpty(key))
                                {
                                    return "无法连接网络！";
                                }
                                else
                                {
                                    string profileName = ssid;
                                    string mac = StringToHex(profileName);
                                    string profileXml = string.Empty;
                                    if (!string.IsNullOrEmpty(key))
                                    {
                                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>{4}</keyType><protected>false</protected><keyMaterial>{5}</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>",
                                            profileName, mac, auth, cipher, keytype, key);
                                    }
                                    else
                                    {
                                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>",
                                            profileName, mac, auth, cipher, keytype);
                                    }
                                    wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                                    bool success = wlanIface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile,
                                        Wlan.Dot11BssType.Any, profileName, 15000);
                                    if (!success)
                                    {
                                        return "连接网络失败！";
                                    }
                                    else
                                    {
                                        return "连接网络成功！";
                                    }
                                }
                            }
                        }
                    }
                }
                return "连接网络失败！";
            }
            catch (Exception e)
            {
                return "连接网络失败,失败原因:" + e.Message;
            }
        }
    }
}
