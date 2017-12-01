using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;

namespace IPCameraMonitorSettings
{
    public class LocalParams
    {
        public string IP = "192.168.0.30";
        public string Mark = "255.255.255.0";
        public string GateWay = "192.168.0.1";
        public int PingTimeOut = 200;
        public int ScanerCount = 8;
    }

    public class GeneralMethod
    {
        public static int TimeOut = 200;//超时设置成200ms

        public static LocalParams GetLocalParams()
        {
            LocalParams ps = new LocalParams();
            try
            {
                string exePath = Assembly.GetExecutingAssembly().Location;
                Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
                if (config != null)
                {
                    if (config.AppSettings.Settings.Count > 0)
                    {
                        string ip = config.AppSettings.Settings["IP"].Value;
                        if (!string.IsNullOrEmpty(ip))
                            ps.IP = ip;
                        string mark = config.AppSettings.Settings["Mask"].Value;
                        if (!string.IsNullOrEmpty(mark))
                            ps.Mark = mark;
                        string gateWay = config.AppSettings.Settings["GateWay"].Value;
                        if (!string.IsNullOrEmpty(gateWay))
                            ps.GateWay = gateWay;
                        string timeOut = config.AppSettings.Settings["PingTimeOut"].Value;
                        if (!string.IsNullOrEmpty(timeOut))
                        {
                            try { ps.PingTimeOut = Convert.ToInt32(timeOut); }
                            catch { ps.PingTimeOut = 200; }
                        }
                        string scanerCount = config.AppSettings.Settings["ScanerCount"].Value;
                        if (!string.IsNullOrEmpty(scanerCount))
                        {
                            try { ps.ScanerCount = Convert.ToInt32(scanerCount); }
                            catch { ps.ScanerCount = 8; }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ps;
        }

        public static bool Ping(string ip)
        {
            try
            {
                if (string.IsNullOrEmpty(ip)) return false;
                System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = pingSender.Send(ip, TimeOut);//第一个参数为ip地址，第二个参数为ping的时间 
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    //ping的通 
                    return true;
                }
                else
                {
                    //ping不通 
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetLocalIPS()
        {
            try
            {
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                List<string> ips = new List<string>();
                for(int i=0;i<addressList.Length;i++)
                {
                    if(addressList[i].AddressFamily == 
                        System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ips.Add(addressList[i].ToString());
                    }
                }

                return ips;
            }
            catch
            {
                return null;
            }
        }

        public static string GetGatewayIP()
        {
            string strGateway = "";
            //获取所有网卡
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //遍历数组
            foreach (var netWork in nics)
            {
                //单个网卡的IP对象
                IPInterfaceProperties ip = netWork.GetIPProperties();
                
                //获取该IP对象的网关
                GatewayIPAddressInformationCollection gateways = ip.GatewayAddresses;
                foreach (var gateWay in gateways)
                {
                    //如果能够Ping通网关
                    if (GeneralMethod.Ping(gateWay.Address.ToString()))
                    {
                        //得到网关地址
                        strGateway = gateWay.Address.ToString();
                        //跳出循环
                        break;
                    }
                }

                //如果已经得到网关地址
                if (strGateway.Length > 0)
                {
                    //跳出循环
                    break;
                }
            }

            return strGateway;
        }

        public static bool ConnectIPC(string ip)
        {
            try
            {
                string pageSource = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + ip);
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.AllowWriteStreamBuffering = false;//禁止缓冲加快载入速度
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");//定义gzip压缩页面支持
                request.ContentType = "application/json; charset=UTF-8";//定义文档类型及编码
                request.AllowAutoRedirect = false;//禁止自动跳转
                                                  //设置User-Agent，伪装成Google Chrome浏览器
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";

                request.Timeout = 9000;//定义请求超时时间为5秒
                request.KeepAlive = false;//启用长连接
                request.Method = "GET";//定义请求方式为GET              
                
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.ContentEncoding.ToLower().Contains("gzip"))//解压
                    {
                        using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))//解压
                    {
                        using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        using (Stream stream = response.GetResponseStream())//原始
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                                //File.WriteAllText(".\\test.txt", pageSource);
                                if (!string.IsNullOrEmpty(pageSource))
                                    return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return false;
        }
    }
}
