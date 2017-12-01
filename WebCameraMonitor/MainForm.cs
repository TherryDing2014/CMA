using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace IPCameraMonitor
{
    public partial class MainForm : Form
    {
        public static string SystemMsg = "";
        private bool IPC1Connected = false;
        private bool IPC2Connected = false;
        private int TryCount = 0;//重试5次

        private string UrlPlayerA = "";
        private string UrlPlayerB = "";
        private SystemSettings systemSettings = new SystemSettings();

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            this.Resize += MainForm_Resize;
            this.DoubleClick += MainForm_DoubleClick;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int width = this.Width / 2;
            this.panel_a.Location = new Point(0,13);
            this.panel_a.Width = width;
            this.panel_a.Height = this.Height - 13;
            this.lb_ipc1.Location = new Point(width/2 - this.lb_ipc1.Width/2, 1);
            this.lb_ipc2.Location = new Point(width + width/2 +
                this.lb_ipc2.Width / 2, 1);
            this.panel_b.Location = new Point(width, 13);
            this.panel_b.Width = width;
            this.panel_b.Height = this.Height - 13;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if(InitSys())
                {
                    InitPlayer();
                }
            }
            catch (System.Exception ex)
            {
                ShowSysError(ex.Message);
            }
        }

        private void MsgInit()
        {
            if (MessageBox.Show("未读取到系统配置,是否现在配置?",
                        "系统配置提示", MessageBoxButtons.YesNo) ==
                        System.Windows.Forms.DialogResult.Yes)
            {
                string setExePath = ".\\系统配置.exe";
                if (File.Exists(setExePath))
                {
                    System.Diagnostics.Process.Start(setExePath);
                    Close();
                }
                else
                {
                    MessageBox.Show("配置工具未找到,请重新安装程序!");
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private bool InitSys()
        {
            if (!File.Exists(SystemSettings.ConfigPath))
            {
                //ShowSysError("未读取到系统配置,请正确配置后重试!");
                MsgInit();
                return false;
            }
            else
            {
                systemSettings = SystemSettings.Read();
                if (systemSettings == null)
                {
                    //ShowSysError("读取系统配置错误,请正确配置后重试!");
                    MsgInit();
                    return false;
                }
            }
            string resMsg = "";
            bool res = APManager.ConnectToSSID(systemSettings.APUser,
                systemSettings.APPass, out resMsg);
            if (!res)
            {
                ShowSysError(resMsg);
                return false;
            }

            return true;
        }

        private void ShowSysError(string msg)
        {
            lb_ipc1.ForeColor = System.Drawing.Color.Red;
            lb_ipc1.Font = new Font("宋体", 20);
            lb_ipc1.Text = msg;
            lb_ipc1.Location = new Point(this.Width / 2 - lb_ipc1.Width / 2,
                this.Height / 2 - lb_ipc1.Height / 2);
            this.lb_ipc2.Visible = false;
        }

        private void InitPlayer()
        {
            if(systemSettings.IPCS == null ||
                systemSettings.IPCS.Count < 1)
            {
                ShowSysError("配置信息错误!");
                return;
            }
            playerA.ScrollBarsEnabled = false;
            playerB.ScrollBarsEnabled = false;

            UrlPlayerA = "http://" + systemSettings.IPCS[0].IP + "/";
            UrlPlayerB = "http://" + systemSettings.IPCS[1].IP + "/";

            if(!string.IsNullOrEmpty(systemSettings.IPCS[0].Des))
                lb_ipc1.Text = "正在连接" + systemSettings.IPCS[0].Des + ",请稍后...";
            else
                lb_ipc1.Text = "正在连接摄像头1请稍后...";
            lb_ipc1.ForeColor = System.Drawing.Color.Green;
            if (!string.IsNullOrEmpty(systemSettings.IPCS[1].Des))
                lb_ipc2.Text = "正在连接" + systemSettings.IPCS[1].Des + ",请稍后...";
            else
                lb_ipc2.Text = "正在连接摄像头2请稍后...";
            lb_ipc2.ForeColor = System.Drawing.Color.Green;
            this.lb_ipc1.Location = new Point(this.playerA.Width / 2 - this.lb_ipc1.Width / 2, 1);
            this.lb_ipc2.Location = new Point(this.Width / 2 + this.playerB.Width / 2 -
                this.lb_ipc2.Width / 2, 1);

            playerA.Navigate(UrlPlayerA);
            playerA.DocumentCompleted += PlayerA_DocumentCompleted;
            playerB.Navigate(UrlPlayerB);
            playerB.DocumentCompleted += PlayerB_DocumentCompleted;

            System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
            tm.Interval = 20000;
            tm.Tick += Tm_Tick;
            tm.Start();
        }

        private void PlayerB_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (playerB.ReadyState == WebBrowserReadyState.Complete)
            {
                if (playerB.Document.Body.Id == "index")
                {
                    IPC2Connected = true;
                    if (!string.IsNullOrEmpty(systemSettings.IPCS[1].Des))
                        lb_ipc2.Text = systemSettings.IPCS[1].Des + "连接成功";
                    else
                        lb_ipc2.Text = "摄像头2连接成功";
                    lb_ipc2.ForeColor = System.Drawing.Color.Green;
                    HtmlElementCollection elms = playerB.Document.GetElementsByTagName("div");
                    foreach (HtmlElement elm in elms)
                    {
                        if (elm.Id == "centerDiv")
                        {
                            elm.Style = "border-image: none;" +
                            "width: 100%; height: 100%;position: relative;";
                        }
                        if (elm.Style == "HEIGHT: 65px; BACKGROUND: #212121; MARGIN-TOP: 0px")
                        {
                            elm.Style = "HEIGHT: 0px; BACKGROUND: #212121; MARGIN-TOP: 0px";
                        }
                        if (elm.GetAttribute("align") == "right")
                        {
                            elm.Style = "display:none";
                        }
                        if (elm.Id == "playSetting" ||
                           elm.Id == "containmsg" ||
                           elm.Id == "top_custom_div" ||
                           elm.Id == "top_logo_div" ||
                           elm.Id == "top_menu_div")
                        {
                            elm.Style = "display:none";
                        }
                        if (elm.Id == "object")
                        {
                            elm.Style = "width: 90%; margin-left: 5%; valign=baseline;position: relative;";
                        }
                        if (elm.Id == "playSetting" ||
                            elm.Id == "stream_rdo" ||
                            elm.Id == "containmsg" ||
                            elm.Id == "bottom_img")
                        {
                            elm.Style = "display:none;";
                        }

                        Thread.Sleep(50);
                        HtmlElement script = playerB.Document.CreateElement("script");
                        script.SetAttribute("type", "text/javascript");
                        string func = "function _func(){" +
                        "$(\"#object\").css(\"height\",$(window).height()-4);" +
                        "$(\"#object\").css(\"width\",$(window).width()-2);" +
                        "$(\"#IPCamera\").css({\"width\": $(window).width()-4,\"height\": $(window).height()-2,\"margin-top\":0});" +
                        "$(\"#object\").css(\"margin-left\",2);" +
                        "}";
                        script.SetAttribute("text", func);
                        HtmlElement head = playerB.Document.Body.AppendChild(script);
                        playerB.Document.InvokeScript("_func");
                    }
                }
                else
                {
                    HtmlElement userName = playerB.Document.GetElementById("loginuserName");
                    if (userName != null)
                    {
                        if (!string.IsNullOrEmpty(systemSettings.IPCS[1].User))
                            userName.InnerText = systemSettings.IPCS[1].User;
                        else
                            userName.InnerText = "admin";
                    }
                    HtmlElement psw = playerB.Document.GetElementById("loginpasswd");
                    if (psw != null)
                    {
                        if (!string.IsNullOrEmpty(systemSettings.IPCS[1].PW))
                            psw.InnerText = systemSettings.IPCS[1].PW;
                        else
                            psw.InnerText = "admin";
                    }
                    HtmlElement loginBtn = playerB.Document.GetElementById("log_login");
                    if (loginBtn != null)
                    {
                        loginBtn.InvokeMember("click");
                    }
                }
            }
        }

        private void PlayerA_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (playerA.ReadyState == WebBrowserReadyState.Complete)
            {
                if(playerA.Document.Body.Id == "index")
                {
                    IPC1Connected = true;
                    if(!string.IsNullOrEmpty(systemSettings.IPCS[0].Des))
                        lb_ipc1.Text = systemSettings.IPCS[0].Des + "连接成功";
                    else
                        lb_ipc1.Text = "摄像头1连接成功";
                    lb_ipc1.ForeColor = System.Drawing.Color.Green;
                    HtmlElementCollection elms = playerA.Document.GetElementsByTagName("div");
                    foreach (HtmlElement elm in elms)
                    {
                        if (elm.Id == "centerDiv")
                        {
                            elm.Style = "border-image: none;" +
                            "width: 100%; height: 100%;position: relative;";
                        }
                        if (elm.Style == "HEIGHT: 65px; BACKGROUND: #212121; MARGIN-TOP: 0px")
                        {
                            elm.Style = "HEIGHT: 0px; BACKGROUND: #212121; MARGIN-TOP: 0px";
                        }
                        if (elm.GetAttribute("align") == "right")
                        {
                            elm.Style = "display:none";
                        }
                        if (elm.Id == "playSetting" ||
                           elm.Id == "containmsg" ||
                           elm.Id == "top_custom_div" ||
                           elm.Id == "top_logo_div" ||
                           elm.Id == "top_menu_div")
                        {
                            elm.Style = "display:none";
                        }
                        if (elm.Id == "object")
                        {
                            elm.Style = "width: 90%; margin-left: 5%; valign=baseline;position: relative;";
                        }
                        if (elm.Id == "playSetting" ||
                            elm.Id == "stream_rdo" ||
                            elm.Id == "containmsg" ||
                            elm.Id == "bottom_img")
                        {
                            elm.Style = "display:none;";
                        }

                        Thread.Sleep(50);
                        HtmlElement script = playerA.Document.CreateElement("script");
                        script.SetAttribute("type", "text/javascript");
                        string func = "function _func(){" +
                        "$(\"#object\").css(\"height\",$(window).height()-4);" +
                        "$(\"#object\").css(\"width\",$(window).width()-2);" +
                        "$(\"#IPCamera\").css({\"width\": $(window).width()-4,\"height\": $(window).height()-2,\"margin-top\":0});" +
                        "$(\"#object\").css(\"margin-left\",2);" +
                        "}";
                        script.SetAttribute("text", func);
                        HtmlElement head = playerA.Document.Body.AppendChild(script);
                        playerA.Document.InvokeScript("_func");
                    }
                }
                else
                {
                    HtmlElement userName = playerA.Document.GetElementById("loginuserName");
                    if (userName != null)
                    {
                        if (!string.IsNullOrEmpty(systemSettings.IPCS[0].User))
                            userName.InnerText = systemSettings.IPCS[0].User;
                        else
                            userName.InnerText = "admin";
                    }
                    HtmlElement psw = playerA.Document.GetElementById("loginpasswd");
                    if (psw != null)
                    {
                        if (!string.IsNullOrEmpty(systemSettings.IPCS[0].PW))
                            psw.InnerText = systemSettings.IPCS[0].PW;
                        else
                            psw.InnerText = "admin";
                    }
                    HtmlElement loginBtn = playerA.Document.GetElementById("log_login");
                    if (loginBtn != null)
                    {
                        loginBtn.InvokeMember("click");
                    }
                }
            }
        }

        private void Tm_Tick(object sender, EventArgs e)
        {
            if(TryCount < 5)
            {
                if(TryCount > 0)//尝试重连
                {
                    if (!IPC1Connected)
                    {
                        if (!string.IsNullOrEmpty(systemSettings.IPCS[0].Des))
                            lb_ipc1.Text = "正在进行" + systemSettings.IPCS[0].Des +
                                "的第" + TryCount + "次重连,请稍后...";
                        else
                            lb_ipc1.Text = "正在进行摄像头1的第" + TryCount
                            + "次重连,请稍后...";
                        lb_ipc1.ForeColor = System.Drawing.Color.Green;
                        playerA.Refresh();
                    }
                    if (!IPC2Connected)
                    {
                        if (!string.IsNullOrEmpty(systemSettings.IPCS[1].Des))
                            lb_ipc2.Text = "正在进行" + systemSettings.IPCS[1].Des +
                                "的第" + TryCount + "次重连,请稍后...";
                        else
                            lb_ipc2.Text = "正在进行摄像头2的第" + TryCount
                            + "次重连,请稍后...";
                        lb_ipc2.ForeColor = System.Drawing.Color.Green;
                        playerB.Refresh();
                    }
                    this.lb_ipc1.Location = new Point(this.playerA.Width / 2 - this.lb_ipc1.Width / 2, 1);
                    this.lb_ipc2.Location = new Point(this.Width / 2 + this.playerB.Width / 2 -
                        this.lb_ipc2.Width / 2, 1);
                }            
                TryCount++;
            }
            else
            {
                if (!IPC1Connected)
                {
                    if(!string.IsNullOrEmpty(systemSettings.IPCS[0].Des))
                        lb_ipc1.Text = systemSettings.IPCS[0].Des + "连接失败,请检查配置后重试!";
                    else
                        lb_ipc1.Text = "摄像头1连接失败,请检查配置后重试!";
                    lb_ipc1.ForeColor = System.Drawing.Color.Red;
                }
                if (!IPC2Connected)
                {
                    if (!string.IsNullOrEmpty(systemSettings.IPCS[1].Des))
                        lb_ipc2.Text = systemSettings.IPCS[1].Des + "连接失败,请检查配置后重试!";
                    else
                        lb_ipc2.Text = "摄像头2连接失败,请检查配置后重试!";
                    lb_ipc2.ForeColor = System.Drawing.Color.Red;
                }
                this.lb_ipc1.Location = new Point(this.playerA.Width / 2 - this.lb_ipc1.Width / 2, 1);
                this.lb_ipc2.Location = new Point(this.Width / 2 + this.playerB.Width / 2 -
                    this.lb_ipc2.Width / 2, 1);
                System.Windows.Forms.Timer tm = (System.Windows.Forms.Timer)sender;
                tm.Stop();
            }
        }

        private void MainForm_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
