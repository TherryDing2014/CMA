namespace IPCameraMonitor
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel_a = new System.Windows.Forms.Panel();
            this.playerA = new System.Windows.Forms.WebBrowser();
            this.panel_b = new System.Windows.Forms.Panel();
            this.playerB = new System.Windows.Forms.WebBrowser();
            this.lb_ipc1 = new System.Windows.Forms.Label();
            this.lb_ipc2 = new System.Windows.Forms.Label();
            this.panel_a.SuspendLayout();
            this.panel_b.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_a
            // 
            this.panel_a.Controls.Add(this.playerA);
            this.panel_a.Location = new System.Drawing.Point(3, 56);
            this.panel_a.Name = "panel_a";
            this.panel_a.Size = new System.Drawing.Size(442, 544);
            this.panel_a.TabIndex = 0;
            // 
            // playerA
            // 
            this.playerA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playerA.Location = new System.Drawing.Point(0, 0);
            this.playerA.MinimumSize = new System.Drawing.Size(20, 20);
            this.playerA.Name = "playerA";
            this.playerA.ScrollBarsEnabled = false;
            this.playerA.Size = new System.Drawing.Size(442, 544);
            this.playerA.TabIndex = 0;
            // 
            // panel_b
            // 
            this.panel_b.Controls.Add(this.playerB);
            this.panel_b.Location = new System.Drawing.Point(451, 56);
            this.panel_b.Name = "panel_b";
            this.panel_b.Size = new System.Drawing.Size(473, 544);
            this.panel_b.TabIndex = 1;
            // 
            // playerB
            // 
            this.playerB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playerB.Location = new System.Drawing.Point(0, 0);
            this.playerB.MinimumSize = new System.Drawing.Size(20, 20);
            this.playerB.Name = "playerB";
            this.playerB.ScrollBarsEnabled = false;
            this.playerB.Size = new System.Drawing.Size(473, 544);
            this.playerB.TabIndex = 0;
            // 
            // lb_ipc1
            // 
            this.lb_ipc1.AutoSize = true;
            this.lb_ipc1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lb_ipc1.Location = new System.Drawing.Point(1, 1);
            this.lb_ipc1.Name = "lb_ipc1";
            this.lb_ipc1.Size = new System.Drawing.Size(47, 12);
            this.lb_ipc1.TabIndex = 2;
            this.lb_ipc1.Text = "摄像头1";
            // 
            // lb_ipc2
            // 
            this.lb_ipc2.AutoSize = true;
            this.lb_ipc2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lb_ipc2.Location = new System.Drawing.Point(449, 1);
            this.lb_ipc2.Name = "lb_ipc2";
            this.lb_ipc2.Size = new System.Drawing.Size(47, 12);
            this.lb_ipc2.TabIndex = 2;
            this.lb_ipc2.Text = "摄像头2";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(936, 612);
            this.Controls.Add(this.lb_ipc2);
            this.Controls.Add(this.lb_ipc1);
            this.Controls.Add(this.panel_b);
            this.Controls.Add(this.panel_a);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "视频监控";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel_a.ResumeLayout(false);
            this.panel_b.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_a;
        private System.Windows.Forms.Panel panel_b;
        private System.Windows.Forms.Label lb_ipc1;
        private System.Windows.Forms.Label lb_ipc2;
        private System.Windows.Forms.WebBrowser playerA;
        private System.Windows.Forms.WebBrowser playerB;
    }
}

