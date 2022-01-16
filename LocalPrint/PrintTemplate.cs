using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LocalPrint.Template;
using LocalPrint.Other;
using LocalPrint.Print;
using Newtonsoft.Json.Serialization;

namespace LocalPrint
{
    public partial class PrintTemplate : Form
    {
        private int n = 0;
        private List<Dictionary<string, string>> list;//表格数据
        private IPrint printObj;//打印对象
        private string jsonPath;  // Application.StartupPath + "\\print.json";
        private string dataFile;//表格文件路径
        private Dictionary<string, ControlEx> ctlPairs;//扩展控件字典,用于左边树形控件节点点击时聚焦到对应控件

        public PrintTemplate()
        {
            InitializeComponent();
        }

        private void initForm(PanelProperty paneProperty)
        {
            panelCanvas.Controls.Clear();//清除原控件,重新添加
            treeView.Nodes.Clear();
            ctlPairs = new Dictionary<string, ControlEx>();
            n = 0;
            TreeNode tnParent = new TreeNode();
            tnParent.Tag = "panelCanvas";
            tnParent.Text = "画布";
            TreeNode tn;
            panelCanvas.Width = (int)(paneProperty.Width * 39f);
            panelCanvas.Height = (int)(paneProperty.Height * 39f);
            if (File.Exists(paneProperty.Img))//读取时先要判读INI文件是否存在
            {
                panelCanvas.BackgroundImage = Image.FromFile(paneProperty.Img);
            }
            else
            {
                panelCanvas.BackgroundImage = null;
            }
            List<PropertyBase> List = paneProperty.List;
            foreach (PropertyBase property in List)
            {
                string type = property.GetType().Name;
                switch (type)
                {
                    case "TextBoxProperty":
                        TextBoxProperty textBoxProperty = property as TextBoxProperty;
                        TextBox TextBox1 = new TextBox();
                        TextBox1.Text = textBoxProperty.Text;
                        TextBox1.Font = new Font(TextBox1.Font.FontFamily, textBoxProperty.Size, TextBox1.Font.Style);
                        TextBox1.Font = new Font(TextBox1.Font, textBoxProperty.Bold ? FontStyle.Bold : FontStyle.Regular);
                        TextBox1.Size = new Size(textBoxProperty.Width, textBoxProperty.Height);
                        TextBox1.Margin = new System.Windows.Forms.Padding(0);
                        TextBox1.Location = new Point(textBoxProperty.Left, textBoxProperty.Top);
                        TextBox1.Multiline = true;
                        TextBox1.Click += new System.EventHandler(obj_Click);
                        //TextBox1.SetMove();

                        TextBox1.Name = "objTextBox" + n;
                        n += 1;

                        panelCanvas.Controls.Add(TextBox1);
                        ctlPairs.Add(TextBox1.Name, new ControlEx(TextBox1));
                        tn = new TreeNode();
                        tn.Tag = TextBox1.Name;
                        tn.Text = "文本" + n;
                        tnParent.Nodes.Add(tn);
                        break;
                    case "PictureBoxProperty":
                        PictureBoxProperty pictureBoxProperty = property as PictureBoxProperty;
                        PictureBox PictureBox1 = new PictureBox();
                        PictureBox1.Tag = pictureBoxProperty.Code;
                        PictureBox1.Image = PictureBoxProperty.CreateBarcodePicture(PictureBox1.Tag.ToString(), pictureBoxProperty.Width, pictureBoxProperty.Height);
                        PictureBox1.Size = new Size(pictureBoxProperty.Width, pictureBoxProperty.Height);
                        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        PictureBox1.Margin = new System.Windows.Forms.Padding(0);
                        PictureBox1.Location = new Point(pictureBoxProperty.Left, pictureBoxProperty.Top);
                        PictureBox1.Click += new System.EventHandler(obj_Click);
                        //PictureBox1.SetMove();

                        PictureBox1.Name = "objPictureBox" + n;
                        n += 1;

                        panelCanvas.Controls.Add(PictureBox1);
                        ctlPairs.Add(PictureBox1.Name, new ControlEx(PictureBox1));
                        tn = new TreeNode();
                        tn.Tag = PictureBox1.Name;
                        tn.Text = "条码" + n;
                        tnParent.Nodes.Add(tn);
                        break;
                    case "image":
                    case "LabelProperty":
                        LabelProperty labelProperty = property as LabelProperty;
                        Label label = new Label();
                        label.BackColor = Color.Black;
                        label.AutoSize = false;
                        label.Text = ".";
                        label.Size = new Size(labelProperty.Width, labelProperty.Height);
                        label.Margin = new System.Windows.Forms.Padding(0);
                        label.Location = new Point(labelProperty.Left, labelProperty.Top);
                        label.Click += new System.EventHandler(obj_Click);
                        //label.SetMove();

                        label.Name = "objLine" + n;
                        n += 1;

                        panelCanvas.Controls.Add(label);
                        ctlPairs.Add(label.Name, new ControlEx(label));
                        tn = new TreeNode();
                        tn.Tag = label.Name;
                        tn.Text = "直线" + n;
                        tnParent.Nodes.Add(tn);
                        break;
                    default: break;
                }
            }

            treeView.Nodes.Add(tnParent);
            treeView.ExpandAll();
        }
        

        //点击控件时显示对应属性
        private void obj_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            propertyGrid.SelectedObject = TemplateHelper.ControlProperty(control);
        }
        //窗口加载
        private void PrintTemplate_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = TemplateHelper.ControlProperty(panelCanvas);
        }
        //打开
        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = false;
            file.Filter = "Json|*.json";
            if (file.ShowDialog() == DialogResult.OK)
            {
                jsonPath = file.FileName;
                string strTemplate = File.ReadAllText(file.FileName);
                MembaseJsonSerializer<PanelProperty> membase = new MembaseJsonSerializer<PanelProperty>();
                PanelProperty panelProperty = membase.FromJson<PanelProperty>(strTemplate);
                initForm(panelProperty);
                treeView.Visible = true;
                propertyGrid.Visible = true;
                panelCanvas.Visible = true;
            }
        }
        //新建
        private void 新建ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ctlPairs = new Dictionary<string, ControlEx>();
            treeView.Visible = true;
            treeView.Nodes.Clear();
            TreeNode tn = new TreeNode();
            tn.Tag = "panelCanvas";
            tn.Text = "画布";
            treeView.Nodes.Add(tn);
            propertyGrid.Visible = true;
            panelCanvas.Visible = true;
            panelCanvas.Controls.Clear();//清除原控件,重新添加
            panelCanvas.Width = 390;
            panelCanvas.Height = 390;
            panelCanvas.BackgroundImage = null;
            treeView.ExpandAll();
        }
        //保存
        private void 保存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PanelProperty panelProperty = new PanelProperty(panelCanvas);
            panelProperty.InitList();
            MembaseJsonSerializer<PanelProperty> membase = new MembaseJsonSerializer<PanelProperty>();
            string json = membase.ToJson(panelProperty);
            if(jsonPath != null)
            {
                File.WriteAllText(jsonPath, json);
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                //设置保存文件对话框的标题
                sfd.Title = "请选择要保存的文件路径";
                //初始化保存目录，默认exe文件目录
                sfd.InitialDirectory = Application.StartupPath;
                //设置保存文件的类型
                sfd.Filter = "Json|*.json";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    jsonPath = sfd.FileName;
                    File.WriteAllText(jsonPath, json);
                }
            }
            
        }
        //另存为
        private void 另存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //设置保存文件对话框的标题
            sfd.Title = "请选择要保存的文件路径";
            //初始化保存目录，默认exe文件目录
            sfd.InitialDirectory = Application.StartupPath;
            //设置保存文件的类型
            sfd.Filter = "Json|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //JsonTemplate template = TemplateHelper.FormToTpl(panelCanvas);
                //获得保存文件的路径
                jsonPath = sfd.FileName;
                //MembaseJsonSerializer<JsonTemplate> membase = new MembaseJsonSerializer<JsonTemplate>();
                //string json = membase.ToJson(template);
                PanelProperty panelProperty = new PanelProperty(panelCanvas);
                panelProperty.InitList();
                MembaseJsonSerializer<PanelProperty> membase = new MembaseJsonSerializer<PanelProperty>();
                string json = membase.ToJson(panelProperty);
                File.WriteAllText(jsonPath, json);
            }
        }
        //添加文本
        private void 文本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox TextBox1 = new TextBox();
            TextBox1.Text = "文本";
            TextBox1.Size = new Size(100, 20);
            TextBox1.Margin = new System.Windows.Forms.Padding(0);
            TextBox1.Location = new Point(10, 10);
            TextBox1.Multiline = true;
            TextBox1.Click += new System.EventHandler(obj_Click);
            //TextBox1.SetMove();

            TextBox1.Name = "objTextBox" + n;
            n += 1;

            panelCanvas.Controls.Add(TextBox1);
            ctlPairs.Add(TextBox1.Name, new ControlEx(TextBox1));
            treeView.BeginUpdate();
            TreeNode tnParent = treeView.Nodes[0];
            TreeNode tn = new TreeNode();
            tn.Tag = TextBox1.Name;
            tn.Text = "文本" + n;
            tnParent.Nodes.Add(tn);
            treeView.EndUpdate();
            propertyGrid.SelectedObject = TemplateHelper.ControlProperty(TextBox1);
            treeView.ExpandAll();
        }
        //添加条码
        private void 条码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox PictureBox1 = new PictureBox(); 
            PictureBox1.Tag = "666666";
            PictureBox1.Image = PictureBoxProperty.CreateBarcodePicture(PictureBox1.Tag.ToString(), 100, 50); 
            PictureBox1.Size = new Size(100, 50);
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox1.Margin = new System.Windows.Forms.Padding(0);
            PictureBox1.Location = new Point(20, 20);
            PictureBox1.Click += new System.EventHandler(obj_Click);
            //PictureBox1.SetMove();

            PictureBox1.Name = "objPictureBox" + n;
            n += 1;

            panelCanvas.Controls.Add(PictureBox1);
            ctlPairs.Add(PictureBox1.Name, new ControlEx(PictureBox1));
            treeView.BeginUpdate();
            TreeNode tnParent = treeView.Nodes[0];
            TreeNode tn = new TreeNode();
            tn.Tag = PictureBox1.Name;
            tn.Text = "条码" + n;
            tnParent.Nodes.Add(tn);
            treeView.EndUpdate();
            propertyGrid.SelectedObject = TemplateHelper.ControlProperty(PictureBox1);
            treeView.ExpandAll();
        }
        //添加横线
        private void 横线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawLine(new Size(100, 2));
        }

        private void 竖线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawLine(new Size(2, 100));
        }
        private void DrawLine(Size size)
        {
            Label label = new Label();
            label.BackColor = Color.Black;
            label.AutoSize = false;
            label.Text = ".";
            label.Size = size;
            label.Margin = new System.Windows.Forms.Padding(0);
            label.Location = new Point(20, 20);
            label.Click += new System.EventHandler(obj_Click);
            //label.SetMove();

            label.Name = "objLine" + n;
            n += 1;

            panelCanvas.Controls.Add(label);
            ctlPairs.Add(label.Name, new ControlEx(label));
            treeView.BeginUpdate();
            TreeNode tnParent = treeView.Nodes[0];
            TreeNode tn = new TreeNode();
            tn.Tag = label.Name;
            tn.Text = "直线" + n;
            tnParent.Nodes.Add(tn);
            treeView.EndUpdate();
            propertyGrid.SelectedObject = TemplateHelper.ControlProperty(label);
            treeView.ExpandAll();
        }
        //单击选中节点
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string key = e.Node.Tag.ToString();
            if(key== "panelCanvas")
            {
                propertyGrid.SelectedObject = TemplateHelper.ControlProperty(panelCanvas);
                panelCanvas.Focus();
            }
            else
            {
                Control control = panelCanvas.Controls.Find(key, false)[0];
                propertyGrid.SelectedObject = TemplateHelper.ControlProperty(control);
                ctlPairs[control.Name].Focus();
            }
        }
        //右键双击删除节点
        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                string key = e.Node.Tag.ToString();
                if (key != "panelCanvas")
                {
                    panelCanvas.Controls.RemoveByKey(key);
                    e.Node.Remove();
                }
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (!printObj.Print(e.Graphics))
                e.HasMorePages = true;//还有需要打印的页
        }

        private void GetPrintObj()
        {

            PanelProperty panelProperty = new PanelProperty(panelCanvas);

            if (dataFile != null && dataFile != "")
            {
                list = ReadTool.ReadFileToList(dataFile);
            }
            printObj = new TemplateParse(panelProperty, list);
        }

        private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetPrintObj();
            this.printDocument1.Print();
        }

        private void 预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetPrintObj();
            this.printPreviewDialog1.ShowDialog();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.pageSetupDialog1.ShowDialog();
        }

        private void 引入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = false;
            file.Filter = "表格|*.xlsx;*.xls;*.csv";
            if (file.ShowDialog() == DialogResult.OK)
            {
                dataFile = file.FileName;
            }
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFile = "";
            list = null;
        }
    }



}
