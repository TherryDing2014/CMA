using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Data;
using TherrySkinLib;
using System.Collections.Generic;
using System;
using System.Windows.Input;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel;
using IPCameraMonitor;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;

namespace IPCameraMonitorSettings
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        WaitingBox waitBox = null;
        List<IPCSettings> settings = new List<IPCSettings>();
        string localIP = "";
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalParams lps = GeneralMethod.GetLocalParams();
                string[] ipss = new string[] { lps.IP };
                string[] marks = new string[] { lps.Mark };
                string[] gateWays = new string[] { "192.168.0.1" };
                APManager.SetIPAddress(ipss, marks, gateWays, null);
                btn_ok.IsEnabled = false;
                if (!File.Exists(SystemSettings.ConfigPath))//未初始化
                {
                    btn_scan.IsEnabled = false;
                }
                else
                {
                    SystemSettings config = SystemSettings.Read();
                    if (config != null)
                    {
                        ShowConfig(config);
                    }
                    //如果网络已经连接
                    string errMsg = "";
                    if (APManager.ConnectToSSID(config.APUser, config.APPass,
                        out errMsg))
                    {
                        List<string> ips = GeneralMethod.GetLocalIPS();
                        localIP = ips[0];
                        ip_begin_h.Text = ips[0].Substring(0, ips[0].LastIndexOf('.') + 1);
                        ip_end_h.Text = ips[0].Substring(0, ips[0].LastIndexOf('.') + 1);
                        btn_scan.IsEnabled = true;
                        btn_connect.Content = "网络已连接";
                        btn_connect.Foreground = new SolidColorBrush(Colors.Green);
                        btn_connect.IsEnabled = false;
                    }
                    else
                    {
                        btn_scan.IsEnabled = false;
                        btn_ok.IsEnabled = false;
                        MsgBox.ShowV2("网络连接异常,请重新连接!", MsgBoxType.Error, this);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowV2(ex.Message,MsgBoxType.Error,this);
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btn_min_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
                this.WindowState = WindowState.Minimized;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            WriteConfig();
        }   
        
        private void ShowConfig(SystemSettings config)
        {
            ap_user.Text = config.APUser;
            ap_password.Password = config.APPass;
            settings.Clear();
            settings = config.IPCS;
            ipcs_settings.ItemsSource = settings;
        }

        private void WriteConfig()
        {
            SystemSettings ss = new SystemSettings();
            ss.APUser = ap_user.Text;
            ss.APPass = ap_password.Password;
            ss.IPCS = new List<IPCSettings>();
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].IsChecked)
                {
                    if (ss.IPCS.Count < 2)
                        ss.IPCS.Add(settings[i]);
                    else
                        break;
                }
            }
            if (ss.IPCS.Count > 0)
            {
                if (SystemSettings.Write(ss))
                    MsgBox.ShowV2("配置成功", MsgBoxType.Info, this);
            }
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            //启动后台线程 
            BackgroundWorker dataWokerThread = new BackgroundWorker();
            dataWokerThread.WorkerReportsProgress = true;
            dataWokerThread.WorkerSupportsCancellation = true;
            dataWokerThread.DoWork += ConnetAP_DoWork;
            dataWokerThread.RunWorkerCompleted += ConnectAP_Completed;
            object[] ps = new object[2];
            ps[0] = ap_user.Text;
            ps[1] = ap_password.Password;
            dataWokerThread.RunWorkerAsync(ps);

            //显示等待窗口
            waitBox = new WaitingBox();
            waitBox.Show(this, "正在连接,请稍后...");
        }

        private void ConnectAP_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                object[] res = (object[])e.Result;
                if((bool)res[0])//连接成功
                {
                    List<string> ips = GeneralMethod.GetLocalIPS();
                    localIP = ips[0];
                    ip_begin_h.Text = ips[0].Substring(0, ips[0].LastIndexOf('.') + 1);
                    ip_end_h.Text = ips[0].Substring(0, ips[0].LastIndexOf('.') + 1);
                    btn_scan.IsEnabled = true;
                    btn_connect.Content = "网络已连接";
                    btn_connect.IsEnabled = false;
                    btn_ok.IsEnabled = true;
                    btn_connect.Foreground = new SolidColorBrush(Colors.Green);
                }
                else//连接失败
                {
                    MsgBox.ShowV2(res[1].ToString(), MsgBoxType.Error, this);
                    btn_scan.IsEnabled = false;
                    btn_ok.IsEnabled = false;
                    btn_connect.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                string errMsg = "";
                if (ex.InnerException != null)
                    errMsg = ex.Message + "(" + ex.InnerException.Message + ")";
                else
                    errMsg = ex.Message;
                MsgBox.ShowV2(errMsg, MsgBoxType.Error, this);
            }
            finally
            {
                if (sender != null)//捕获异常后手动关闭线程
                {
                    BackgroundWorker worker = sender as BackgroundWorker;
                    if (worker.WorkerSupportsCancellation && !worker.CancellationPending)
                        worker.CancelAsync();
                    worker.Dispose();
                }
                if (waitBox != null)
                {
                    waitBox.Close();
                }
            }
        }

        private void ConnetAP_DoWork(object sender, DoWorkEventArgs e)
        {
            object []args = (object[])e.Argument;
            string errMsg = "";
            bool state = APManager.ConnectToSSID(args[0].ToString(),
                args[1].ToString(),out errMsg);
            object[] res = new object[2];
            res[0] = state;
            res[1] = errMsg;
            e.Result = res;
        }

        private void btn_scan_Click(object sender, RoutedEventArgs e)
        {
            //启动后台线程 
            BackgroundWorker dataWokerThread = new BackgroundWorker();
            dataWokerThread.WorkerReportsProgress = true;
            dataWokerThread.WorkerSupportsCancellation = true;
            dataWokerThread.DoWork += ScanDev_DoWork; ;
            dataWokerThread.RunWorkerCompleted += ScanDev_Completed;
            object[] ps = new object[3];
            ps[0] = ip_begin_h.Text;
            ps[1] = ip_begin_t.Text;
            ps[2] = ip_end_t.Text;
            dataWokerThread.RunWorkerAsync(ps);

            //显示等待窗口
            waitBox = new WaitingBox();
            waitBox.Show(this, "正在扫描,请稍后...");
        }

        private void ScanDev_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                List<string> res = (List<string>)e.Result;
                if (res.Count > 0)
                {
                    ipcs_settings.ItemsSource = null;
                    settings.Clear();

                    for (int i = 0; i < res.Count; i++)
                    {
                        if (i < 2)
                        {
                            IPCSettings ipcs = new IPCSettings(true, res[i],
                            SystemSettings.DefaultPort, "通道" + (i + 1), "", "");
                            settings.Add(ipcs);
                        }
                        else
                        {
                            IPCSettings ipcs = new IPCSettings(false, res[i],
                            SystemSettings.DefaultPort, "通道" + (i + 1), "", "");
                            settings.Add(ipcs);
                        }
                    }
                    ipcs_settings.ItemsSource = settings;
                    ipcs_settings.Items.Refresh();
                    btn_ok.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                string errMsg = "";
                if (ex.InnerException != null)
                    errMsg = ex.Message + "(" + ex.InnerException.Message + ")";
                else
                    errMsg = ex.Message;
                MsgBox.ShowV2(errMsg, MsgBoxType.Error, this);
            }
            finally
            {
                if (sender != null)//捕获异常后手动关闭线程
                {
                    BackgroundWorker worker = sender as BackgroundWorker;
                    if (worker.WorkerSupportsCancellation && !worker.CancellationPending)
                        worker.CancelAsync();
                    worker.Dispose();
                }
                if (waitBox != null)
                {
                    waitBox.Close();
                }
            }
        }

        private void ScanDev_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] args = (object[])e.Argument;
            int begin = Convert.ToInt32(args[1]);
            int end = Convert.ToInt32(args[2]);
            List<string> ips = new List<string>();
            string ipHeader = args[0].ToString();
            ScanDeviceManager sdm = new ScanDeviceManager();
            sdm.Run(ipHeader,begin,end);
            while(true)
            {
                if(sdm.Finish)
                {
                    string[] res = sdm.IPS.Split(';');
                    for(int i=0;i<res.Length;i++)
                    {
                        if (!string.IsNullOrEmpty(res[i]))
                        {
                            if (GeneralMethod.ConnectIPC(res[i]))
                            {
                                ips.Add(res[i]);
                            }
                        }
                    }
                    break;
                }
                Thread.Sleep(500);
            }

            e.Result = ips;
        }

        private void ap_user_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(btn_connect != null)
                btn_connect.IsEnabled = true;
        }

        private void ap_password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(btn_connect != null)
                btn_connect.IsEnabled = true;
        }

        private void ipcs_settings_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            if(btn_ok != null)
                btn_ok.IsEnabled = true;
        }

        private void IPC_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox ps = (PasswordBox)sender;
            if(ps != null)
            {
                if (ipcs_settings.SelectedIndex != -1)
                {
                    IPCSettings drw = ipcs_settings.CurrentItem as IPCSettings;
                    if (drw != null)
                    {
                        drw.PW = ps.Password;
                        btn_ok.IsEnabled = true;
                    }
                }
            }
        }
    }
}
