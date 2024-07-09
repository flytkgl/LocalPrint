using LocalPrint.Print;
using LocalPrint.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LocalPrint.Other;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string Port = "23333";//默认端口号
        private string strIniFilePath = Application.StartupPath + "\\config.ini";//获取INI文件路径

        private void Form1_Load(object sender, EventArgs e)
        {
#if (!DEBUG)
            WindowState = FormWindowState.Minimized;//隐藏掉主窗口
            ShowInTaskbar = false;
#endif
            string Printer = "";//打印机
            string Path = Application.StartupPath;//模板目录
            string Open = "1";
            if (File.Exists(strIniFilePath))//读取时先要判读INI文件是否存在
            {
                IniFileHelper iniFileHelper = new IniFileHelper(strIniFilePath);
                string section = "config";
                Open = iniFileHelper.ContentValue(section, "Open");
                Port = iniFileHelper.ContentValue(section, "Port");
                Path = iniFileHelper.ContentValue(section, "Path");
                if(Path=="")
                    Path = Application.StartupPath;//默认模板目录

                Printer = iniFileHelper.ContentValue(section, "Printer");
            }
            else
            {
                IniFileHelper iniFileHelper = new IniFileHelper(strIniFilePath);
                string section = "config";
                iniFileHelper.WriteIniString(section, "Open", Open);
                iniFileHelper.WriteIniString(section, "Port", Port);
                iniFileHelper.WriteIniString(section, "Path", "");
                iniFileHelper.WriteIniString(section, "Printer", Printer);
            }


            WebServer webServer = new WebServer();
            if (!webServer.RunWeb("http://localhost:"+ Port+"/"))
            {
                MessageBox.Show(webServer.Err);
                Application.Exit();
            }
            else
            {
                webServer.AddHandler("/print", new PrintHandler());

                notifyIcon1.Text = "本地打印服务 V1.1";
                notifyIcon1.BalloonTipText = "本地打印服务已启动！";
                notifyIcon1.ShowBalloonTip(0);
                PrintHandler.Printer = Printer;
                PrintHandler.Path = Path;
                if (Open == "1")
                {
                    var template = new PrintTemplate();
                    template.ShowDialog();
                }
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //设置打印机
        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var printers = new PrinterSet_FM(strIniFilePath);
            //printers.Printer = PrintHandler.Printer;
            if(printers.ShowDialog() == DialogResult.OK)
            {
                PrintHandler.Printer = printers.Printer;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //测试代码
            string json = textBoxJson.Text;
            var web = new WebClient();
            web.Encoding = Encoding.UTF8;
            web.UploadString("http://localhost:" + Port + "//print", "POST", json);

        }

        private void 模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var template = new PrintTemplate();
            template.ShowDialog();
        }
    }
}
