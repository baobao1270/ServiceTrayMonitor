using System;
using System.Windows.Forms;

namespace Joseph.ServiceTrayMonitor
{
    partial class HostedNotifyIcon
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostedNotifyIcon));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemStartAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStopAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemShowMain = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator,
            this.toolStripMenuItemStartAll,
            this.toolStripMenuItemStopAll,
            this.toolStripMenuItemShowMain,
            this.toolStripMenuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(158, 114);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(154, 6);
            // 
            // toolStripMenuItemStartAll
            // 
            this.toolStripMenuItemStartAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemStartAll.Image")));
            this.toolStripMenuItemStartAll.Name = "toolStripMenuItemStartAll";
            this.toolStripMenuItemStartAll.Size = new System.Drawing.Size(157, 26);
            this.toolStripMenuItemStartAll.Text = "启动所有";
            // 
            // toolStripMenuItemStopAll
            // 
            this.toolStripMenuItemStopAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemStopAll.Image")));
            this.toolStripMenuItemStopAll.Name = "toolStripMenuItemStopAll";
            this.toolStripMenuItemStopAll.Size = new System.Drawing.Size(157, 26);
            this.toolStripMenuItemStopAll.Text = "停止所有";
            // 
            // toolStripMenuItemShowMain
            // 
            this.toolStripMenuItemShowMain.Name = "toolStripMenuItemShowMain";
            this.toolStripMenuItemShowMain.Size = new System.Drawing.Size(157, 26);
            this.toolStripMenuItemShowMain.Text = "显示主界面";
            this.toolStripMenuItemShowMain.Click += new System.EventHandler(this.ToolStripMenuItemShowMain_OnClick);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(157, 26);
            this.toolStripMenuItemExit.Text = "退出";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_OnClick);
            // 
            // timer
            // 
            this.timer.Interval = 3000;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // HostedNotifyIcon
            // 
            this.Text = " ";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem toolStripMenuItemShowMain;
        private ToolStripMenuItem toolStripMenuItemExit;
        public ContextMenuStrip contextMenuStrip;
        public NotifyIcon notifyIcon;
        private Timer timer;
        public ToolStripMenuItem toolStripMenuItemStartAll;
        public ToolStripMenuItem toolStripMenuItemStopAll;
    }
}
