using LocalPrint.Print;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using LocalPrint.Other;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint.Template
{
    //示例打印类，打印这一块也可以封装成一个简单的格式，这是另一个问题了，有空再封装了
    class TemplateParse : IPrint
    {
        public string strTemplate { set; get; }
        public List<Dictionary<string, string>> list { set; get; }
        private int currentPageIndex;
        private int pageCount;

        public TemplateParse(string strTemplate, List<Dictionary<string, string>> list)
        {
            this.strTemplate = strTemplate;
            this.list = list;
            this.currentPageIndex = 0;
            if(list == null)
            {
                this.pageCount = 0;
            }
            else
            {
                this.pageCount = list.Count;
            }
        }
        public TemplateParse(PanelProperty panelProperty, List<Dictionary<string, string>> list)
        {
            panelProperty.InitList();
            MembaseJsonSerializer<PanelProperty> membase = new MembaseJsonSerializer<PanelProperty>();
            this.strTemplate = membase.ToJson(panelProperty);
            this.list = list;
            this.currentPageIndex = 0;
            if (list == null)
            {
                this.pageCount = 0;
            }
            else
            {
                this.pageCount = list.Count;
            }
        }

        public bool Print(Graphics g)
        {
            string str = strTemplate;
            if (list != null && pageCount>0)
            {
                Dictionary<string, string> dict = list[currentPageIndex];

                foreach (var kv in dict)
                {
                    str = str.Replace("{{" + kv.Key + "}}", kv.Value);
                }
            }
            MembaseJsonSerializer<PanelProperty> membase = new MembaseJsonSerializer<PanelProperty>();
            PanelProperty panelProperty = membase.FromJson<PanelProperty>(str);

            foreach (PropertyBase property in panelProperty.List)
            {
                string type = property.GetType().Name;
                switch (type)
                {
                    case "TextBoxProperty":
                        TextBoxProperty textBoxProperty = property as TextBoxProperty;
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center; //居中
                        Rectangle rectangle = new Rectangle(textBoxProperty.Left, textBoxProperty.Top, textBoxProperty.Width, textBoxProperty.Height);
                        var font = new Font("宋体", textBoxProperty.Size, textBoxProperty.Bold? FontStyle.Bold:FontStyle.Regular);
                        var brush = new SolidBrush(Color.Black);
                        g.DrawString(textBoxProperty.Text, font, brush, rectangle, format);
                        break;
                    case "PictureBoxProperty":
                        PictureBoxProperty pictureBoxProperty = property as PictureBoxProperty;
                        if (!string.IsNullOrEmpty(pictureBoxProperty.Code))
                        {
                            //绘制打印图片
                            g.DrawImage(PictureBoxProperty.CreateBarcodePicture(pictureBoxProperty.Code, pictureBoxProperty.Width, pictureBoxProperty.Height), pictureBoxProperty.Left, pictureBoxProperty.Top, pictureBoxProperty.Width, pictureBoxProperty.Height);
                        }
                        break;
                    case "image":
                    case "LabelProperty":
                        LabelProperty labelProperty = property as LabelProperty;
                        Pen myPen = new Pen(Color.Black, 1);
                        if (labelProperty.Width > labelProperty.Height)
                        {
                            labelProperty.Height = 0;
                        }
                        else
                        {
                            labelProperty.Width = 0;
                        }
                        g.DrawLine(myPen, labelProperty.Left, labelProperty.Top, labelProperty.Left + labelProperty.Width, labelProperty.Top + labelProperty.Height);
                        break;
                    default: break;
                }
            }
            g.Dispose();

            this.currentPageIndex++;      //加新页
            if (currentPageIndex < pageCount)
            {
                return false;//没打印完
            }
            else
            {
                this.currentPageIndex = 0;
                return true;//打印完毕
            }
        }
    }


}
