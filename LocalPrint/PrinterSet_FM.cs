using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using LocalPrint.Other;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint
{
    public partial class PrinterSet_FM : Form
    {
        public PrinterSet_FM(string strIniFilePath)
        {
            this.strIniFilePath = strIniFilePath;
            InitializeComponent();
        }
        public string Printer;
        public string strIniFilePath;

        private void PrinterSet_FM_Load(object sender, EventArgs e)
        {
            foreach (var pri in PrinterSettings.InstalledPrinters)
                comboPrint.Items.Add(pri);

            //读取配置
            IniFileHelper iniFileHelper = new IniFileHelper(strIniFilePath);
            string section = "config";
            textPort.Text = iniFileHelper.ContentValue(section, "Port");
            textPath.Text = iniFileHelper.ContentValue(section, "Path");
            comboPrint.Text = iniFileHelper.ContentValue(section, "Printer");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //写入配置
            IniFileHelper iniFileHelper = new IniFileHelper(strIniFilePath);
            string section = "config";
            iniFileHelper.WriteIniString(section, "Port", textPort.Text.Trim());
            iniFileHelper.WriteIniString(section, "Path", textPath.Text.Trim());
            iniFileHelper.WriteIniString(section, "Printer", comboPrint.Text.Trim());

            Printer = comboPrint.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
