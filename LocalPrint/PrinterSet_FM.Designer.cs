namespace LocalPrint
{
    partial class PrinterSet_FM
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrinterSet_FM));
            this.labelPrint = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboPrint = new System.Windows.Forms.ComboBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.textPort = new System.Windows.Forms.TextBox();
            this.textPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelPrint
            // 
            this.labelPrint.AutoSize = true;
            this.labelPrint.Location = new System.Drawing.Point(155, 15);
            this.labelPrint.Name = "labelPrint";
            this.labelPrint.Size = new System.Drawing.Size(41, 12);
            this.labelPrint.TabIndex = 5;
            this.labelPrint.Text = "打印机";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(308, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboPrint
            // 
            this.comboPrint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPrint.FormattingEnabled = true;
            this.comboPrint.Location = new System.Drawing.Point(202, 12);
            this.comboPrint.Name = "comboPrint";
            this.comboPrint.Size = new System.Drawing.Size(181, 20);
            this.comboPrint.TabIndex = 3;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(12, 15);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(53, 12);
            this.labelPort.TabIndex = 6;
            this.labelPort.Text = "运行端口";
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(68, 12);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(80, 21);
            this.textPort.TabIndex = 7;
            this.textPort.Text = "23333";
            // 
            // textPath
            // 
            this.textPath.Location = new System.Drawing.Point(68, 39);
            this.textPath.Name = "textPath";
            this.textPath.Size = new System.Drawing.Size(315, 21);
            this.textPath.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "模板目录";
            // 
            // labelNotes
            // 
            this.labelNotes.AutoSize = true;
            this.labelNotes.Location = new System.Drawing.Point(14, 68);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(353, 12);
            this.labelNotes.TabIndex = 10;
            this.labelNotes.Text = "注:修改端口需重启程序才生效,模板目录留空会默认程序当前目录";
            // 
            // PrinterSet_FM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 128);
            this.Controls.Add(this.labelNotes);
            this.Controls.Add(this.textPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textPort);
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.labelPrint);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboPrint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrinterSet_FM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.Load += new System.EventHandler(this.PrinterSet_FM_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPrint;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboPrint;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.TextBox textPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelNotes;
    }
}