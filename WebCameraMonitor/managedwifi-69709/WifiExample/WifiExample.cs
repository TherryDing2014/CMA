using NativeWifi;
using System;
using System.Text;

namespace WifiExample
{
    class Program
    {
        /// <summary>
        /// Converts a 802.11 SSID to a string.
        /// </summary>
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.UTF8.GetString( ssid.SSID, 0, (int) ssid.SSIDLength );
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
        public static string StringToHex(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.Default.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString().ToUpper());

        }


        static void ConnectToSSID(WIFISSID ssid,string key)
        {
            try
            {
                String auth = string.Empty;
                String cipher = string.Empty;
                bool isNoKey = false;
                String keytype = string.Empty;
                //Console.WriteLine("》》》《《" + ssid.dot11DefaultAuthAlgorithm + "》》对比《《" + "Wlan.Dot11AuthAlgorithm.RSNA_PSK》》");
                switch (ssid.dot11DefaultAuthAlgorithm)
                {
                    case "IEEE80211_Open":
                        auth = "open"; break;
                    case "RSNA":
                        auth = "WPA2PSK"; break;
                    case "RSNA_PSK":
                        //Console.WriteLine("电子设计wifi：》》》");
                        auth = "WPA2PSK"; break;
                    case "WPA":
                        auth = "WPAPSK"; break;
                    case "WPA_None":
                        auth = "WPAPSK"; break;
                    case "WPA_PSK":
                        auth = "WPAPSK"; break;
                }
                switch (ssid.dot11DefaultCipherAlgorithm)
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

                    Console.WriteLine(">>>>>>>>>>>>>>>>>无法连接网络！");
                    return;
                }
                else if (!isNoKey && string.IsNullOrEmpty(key))
                {
                    Console.WriteLine("无法连接网络！");
                    return;
                }
                else
                {
                    //string profileName = ssid.profileNames; // this is also the SSID 
                    string profileName = ssid.SSID;
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

                    ssid.wlanInterface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);

                    bool success = ssid.wlanInterface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName, 15000);
                    if (!success)
                    {
                        Console.WriteLine("连接网络失败！");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("连接网络成功！");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("连接网络失败！");
                return;
            }
        }

        static void Main( string[] args )
        {
            //WlanClient client = new WlanClient();
            //foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            //{
            //    //当前连接的网络
            //    //Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
            //    //foreach (Wlan.WlanAvailableNetwork network in networks)
            //    //{
            //    //    if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected && 
            //    //        wlanIface.CurrentConnection.isState == Wlan.WlanInterfaceState.Connected)
            //    //    {
            //    //        Console.WriteLine(wlanIface.CurrentConnection.profileName);
            //    //    }
            //    //}

            //    Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
            //    foreach (Wlan.WlanAvailableNetwork network in networks)
            //    {
            //        WIFISSID targetSSID = new WIFISSID();

            //        targetSSID.wlanInterface = wlanIface;
            //        targetSSID.wlanSignalQuality = (int)network.wlanSignalQuality;
            //        targetSSID.SSID = GetStringForSSID(network.dot11Ssid);
            //        //targetSSID.SSID = Encoding.Default.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength);
            //        targetSSID.dot11DefaultAuthAlgorithm = network.dot11DefaultAuthAlgorithm.ToString();
            //        targetSSID.dot11DefaultCipherAlgorithm = network.dot11DefaultCipherAlgorithm.ToString();
            //        Console.WriteLine(targetSSID.SSID + ":" + targetSSID.wlanSignalQuality);
            //        if(targetSSID.SSID == "sany2")
            //        {
            //            ConnectToSSID(targetSSID, "sanysany");
            //            break;
            //        }
            //    }
            //}
            string res = APManager.ConnectToSSID("sany2", "we");
        }
    }

    class WIFISSID
    {
        public string SSID = "NONE";
        public string dot11DefaultAuthAlgorithm = "";
        public string dot11DefaultCipherAlgorithm = "";
        public bool networkConnectable = true;
        public string wlanNotConnectableReason = "";
        public int wlanSignalQuality = 0;
        public WlanClient.WlanInterface wlanInterface = null;
    }
}
