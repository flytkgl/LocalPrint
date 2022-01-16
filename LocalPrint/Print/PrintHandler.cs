using LocalPrint.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;
using LocalPrint.Template;
using System.IO;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint.Print
{
    class PrintHandler : IHttpHandler
    {
        IPrint printObj;
        PrintDocument pdoc;
        public static string Printer="";
        public static string Path = "";
        public PrintHandler()
        {
            pdoc = new PrintDocument();
            pdoc.PrintPage += Pdoc_PrintPage;
        }

        private void Pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (!printObj.Print(e.Graphics))
                e.HasMorePages = true;//还有需要打印的页
        }

        private string Print(string data)
        {
            try
            {
                if (data == "")
                    throw new Exception("打印内容为空！");

                if (Printer != "")//使用设置打印机，否则为默认打印机
                    pdoc.PrinterSettings.PrinterName = Printer;

                WebJson webJson = JsonConvert.DeserializeObject<WebJson>(data);
                string strTemplate;
                if (webJson.type == 0)
                {
                    strTemplate = webJson.template;
                }
                else
                {
                    string filepath;
                    if (webJson.template == "")
                    {
                        filepath = Path + "\\print.json";//本地默认模板
                    }
                    else
                    {
                        filepath = Path + "\\" + webJson.template + ".json";//
                    }

                    if (File.Exists(filepath))//读取时先要判读本地模板文件是否存在
                    {
                        strTemplate = File.ReadAllText(filepath);
                    }
                    else
                    {
                        return "模板不存在";
                    }
                }

                printObj = new TemplateParse(strTemplate, webJson.list);
                pdoc.Print();

                return "打印成功";
            }
            catch (Exception ex)
            {
                return "打印出错：" + ex.Message;
            }
        }
        public string Handler(string txt)
        {
            var ret = Print(txt);
            return ret;
        }
    }
}
