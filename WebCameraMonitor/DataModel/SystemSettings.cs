using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace IPCameraMonitor
{
    public class SystemSettings
    {
        public const string ConfigPath = ".\\Config.json";
        public const string DefaultPort = "8081";
        public string APUser { get; set; }
        public string APPass { get; set; }
        public List<IPCSettings> IPCS { get; set; }

        public static SystemSettings Read()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return null;
                string strData = File.ReadAllText(ConfigPath);
                if (string.IsNullOrEmpty(strData)) return null;
                else
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    SystemSettings objData = jss.Deserialize<SystemSettings>(strData);
                    return objData;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static void Write(SystemSettings data)
        {
            try
            {
                if (data == null) return;
                else
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    string strData = jss.Serialize(data);
                    if (!string.IsNullOrEmpty(strData))
                        File.WriteAllText(ConfigPath,strData);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class IPCSettings : INotifyPropertyChanged
    {
        //是否启用
        private bool _IsChecked = true;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        //ip地址
        private string _IP = "";
        public string IP
        {
            get { return _IP; }
            set
            {
                _IP = value;
                NotifyPropertyChanged("IP");
            }
        }

        //端口号
        private string _Port = "";
        public string Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
                NotifyPropertyChanged("Port");
            }
        }

        //通道描述
        private string _Des = "";
        public string Des
        {
            get { return _Des; }
            set
            {
                _Des = value;
                NotifyPropertyChanged("Des");
            }
        }

        //User
        private string _User = "";
        public string User
        {
            get { return _User; }
            set
            {
                _User = value;
                NotifyPropertyChanged("User");
            }
        }

        //password
        private string _PW = "";
        public string PW
        {
            get { return _PW; }
            set
            {
                _PW = value;
                NotifyPropertyChanged("PW");
            }
        }

        public IPCSettings()
        {

        }

        public IPCSettings(bool isChecked,string ip,string port,string des,
            string user,string pass)
        {
            _IsChecked = isChecked;
            _IP = ip;
            _Port = port;
            _Des = des;
            _User = user;
            _PW = pass;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
