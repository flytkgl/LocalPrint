using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LocalPrint.Other
{
    internal class ControlEx
    {
        #region private  

        private const int MIN_SIZE = 2; //对控件缩放的最小值   
        private const int BOX_SIZE = 7;  //调整大小触模柄方框大小   

        public bool _IsCtrlKey = false;
        private MaskedTextBox _textbox;
        private Control _MControl = null;
        private bool _IsMouseDown = false;
        private Point _oPointClicked;
        private Color BOX_COLOR = Color.White;
        private LinkLabel[] _lbl = new LinkLabel[8];
        private int _startl, _startt, _startw, _starth;
        private bool _dragging;
        private Cursor[] _arrArrow = new Cursor[] {Cursors.SizeNWSE, Cursors.SizeNS,
                                                    Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeNS,
                                                    Cursors.SizeNESW, Cursors.SizeWE};

        #endregion

        #region 构造函数  

        /// <summary>   
        /// 构造控件拖动对象   
        /// </summary>   
        /// <param name="moveControl">需要拖动的控件 </param>   
        public ControlEx(Control moveControl)
        {
            //   
            // TODO: 在此处添加构造函数逻辑   
            //   
            _MControl = moveControl;
            _MControl.MouseDown += new MouseEventHandler(this.Control_MouseDown);
            _MControl.MouseUp += new MouseEventHandler(this.Control_MouseUp);
            _MControl.MouseMove += new MouseEventHandler(this.Control_MouseMove);
            _MControl.Click += new System.EventHandler(this.Control_Click);

            //构造8个调整大小触模柄   
            for (int i = 0; i < 8; i++)
            {
                _lbl[i] = new LinkLabel();
                _lbl[i].TabIndex = i;
                _lbl[i].FlatStyle = 0;
                _lbl[i].BorderStyle = BorderStyle.FixedSingle;
                _lbl[i].BackColor = BOX_COLOR;
                _lbl[i].Cursor = _arrArrow[i];
                _lbl[i].Text = "";
                _lbl[i].BringToFront();
                _lbl[i].MouseDown += new MouseEventHandler(this.handle_MouseDown);
                _lbl[i].MouseMove += new MouseEventHandler(this.handle_MouseMove);
                _lbl[i].MouseUp += new MouseEventHandler(this.handle_MouseUp);
            }

            CreateTextBox();
            Create();

            //Control_Click((object)sender, (System.EventArgs)e);   
        }

        public void Focus()
        {
            _textbox.Focus();
            for (int i = 0; i < _MControl.Parent.Controls.Count; i++)
            {
                if (_MControl.Parent.Controls[i].Text.Trim().Length == 0 && _MControl.Parent.Controls[i] is LinkLabel)
                {
                    _MControl.Parent.Controls[i].Visible = false;
                }
            }
            ShowHandles();
        }

        #endregion

        #region 需拖动控件键盘事件  

        private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 37 || e.KeyValue == 38 || e.KeyValue == 39 || e.KeyValue == 40)
            {
                if (e.KeyValue == 37)
                    _MControl.Left -= 1;
                if (e.KeyValue == 38)
                    _MControl.Top -= 1;
                if (e.KeyValue == 39)
                    _MControl.Left += 1;
                if (e.KeyValue == 40)
                    _MControl.Top += 1;
                MoveHandles();
                ControlLocality();
                _MControl.Visible = true;
            }

            if (e.KeyValue == 46)
            {
                for (int i = 0; i < 8; i++)
                {
                    _MControl.Parent.Controls.Remove(_lbl[i]);
                }
                _MControl.Parent.Controls.Remove(_MControl);
                _textbox.Parent.Controls.Remove(_textbox);
            }

            if (e.KeyValue == 17)
            {
                _IsCtrlKey = true;
                //MessageBox.Show("a");   
            }
        }

        #endregion

        #region 需拖动控件鼠标事件  

        private void Control_Click(object sender, System.EventArgs e)
        {
            _textbox.Focus();
            _MControl = (sender as Control);
            MoveHandles();

            if (_IsCtrlKey == false)
            {
                for (int i = 0; i < _MControl.Parent.Controls.Count; i++)
                {
                    if (_MControl.Parent.Controls[i].Text.Trim().Length == 0 && _MControl.Parent.Controls[i] is LinkLabel)
                    {
                        _MControl.Parent.Controls[i].Visible = false;
                    }
                }
            }
        }
        private void Control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _IsMouseDown = true;
            _oPointClicked = new Point(e.X, e.Y);
            for (int i = 0; i < 8; i++)
            {
                _MControl.Parent.Controls.Add(_lbl[i]);
                _lbl[i].BringToFront();
            }
            HideHandles();
        }

        private void Control_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _IsMouseDown = false;
            MoveHandles();
            ShowHandles();
            _MControl.Visible = true;
            _MControl.Focus();
        }

        private void Control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_IsMouseDown)
            {
                int l = _MControl.Left + (e.X - _oPointClicked.X);
                int t = _MControl.Top + (e.Y - _oPointClicked.Y);
                int w = _MControl.Width;
                int h = _MControl.Height;
                l = (l < 0) ? 0 : ((l + w > _MControl.Parent.ClientRectangle.Width) ?
                    _MControl.Parent.ClientRectangle.Width - w : l);
                t = (t < 0) ? 0 : ((t + h > _MControl.Parent.ClientRectangle.Height) ?
                    _MControl.Parent.ClientRectangle.Height - h : t);
                _MControl.Left = l;
                _MControl.Top = t;
                ControlLocality();
            }
            //_MControl.Cursor=Cursors.SizeAll;   
        }

        #endregion
        #region 调整大小触模柄鼠标事件  

        private void handle_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _startl = _MControl.Left;
            _startt = _MControl.Top;
            _startw = _MControl.Width;
            _starth = _MControl.Height;
            HideHandles();
        }
        // 通过触模柄调整控件大小   
        //    0  1  2   
        //  7      3   
        //  6  5  4   
        private void handle_MouseMove(object sender, MouseEventArgs e)
        {
            int l = _MControl.Left;
            int w = _MControl.Width;
            int t = _MControl.Top;
            int h = _MControl.Height;
            if (_dragging)
            {
                switch (((LinkLabel)sender).TabIndex)
                {
                    //l算法：控件左边X坐标 ＋ 鼠标在触模柄X坐标 < 控件左边X坐标 ＋ 父控件宽度 - 控件大小 ？控件左边X坐标 ＋ 鼠标在触模柄X坐标 ：控件左边X坐标 ＋ 父控件宽度 - 控件大小    
                    //t算法：   
                    //w算法：   
                    //h算法：   
                    case 0: // _dragging top-left sizing box   
                        l = _startl + e.X < _startl + _startw - MIN_SIZE ? _startl + e.X : _startl + _startw - MIN_SIZE;
                        t = _startt + e.Y < _startt + _starth - MIN_SIZE ? _startt + e.Y : _startt + _starth - MIN_SIZE;
                        w = _startl + _startw - _MControl.Left;
                        h = _startt + _starth - _MControl.Top;
                        break;
                    case 1: // _dragging top-center sizing box   
                        t = _startt + e.Y < _startt + _starth - MIN_SIZE ? _startt + e.Y : _startt + _starth - MIN_SIZE;
                        h = _startt + _starth - _MControl.Top;
                        break;
                    case 2: // _dragging top-right sizing box   
                        w = _startw + e.X > MIN_SIZE ? _startw + e.X : MIN_SIZE;
                        t = _startt + e.Y < _startt + _starth - MIN_SIZE ? _startt + e.Y : _startt + _starth - MIN_SIZE;
                        h = _startt + _starth - _MControl.Top;
                        break;
                    case 3: // _dragging right-middle sizing box   
                        w = _startw + e.X > MIN_SIZE ? _startw + e.X : MIN_SIZE;
                        break;
                    case 4: // _dragging right-bottom sizing box   
                        w = _startw + e.X > MIN_SIZE ? _startw + e.X : MIN_SIZE;
                        h = _starth + e.Y > MIN_SIZE ? _starth + e.Y : MIN_SIZE;
                        break;
                    case 5: // _dragging center-bottom sizing box   
                        h = _starth + e.Y > MIN_SIZE ? _starth + e.Y : MIN_SIZE;
                        break;
                    case 6: // _dragging left-bottom sizing box   
                        l = _startl + e.X < _startl + _startw - MIN_SIZE ? _startl + e.X : _startl + _startw - MIN_SIZE;
                        w = _startl + _startw - _MControl.Left;
                        h = _starth + e.Y > MIN_SIZE ? _starth + e.Y : MIN_SIZE;
                        break;
                    case 7: // _dragging left-middle sizing box   
                        l = _startl + e.X < _startl + _startw - MIN_SIZE ? _startl + e.X : _startl + _startw - MIN_SIZE;
                        w = _startl + _startw - _MControl.Left;
                        break;
                }
                l = (l < 0) ? 0 : l;
                t = (t < 0) ? 0 : t;
                _MControl.SetBounds(l, t, w, h);
            }
        }

        private void handle_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
            MoveHandles();
            ShowHandles();
        }

        #endregion

        #region private方法  

        private void Create()
        {
            //_IsMouseDown = true;   
            //_oPointClicked = new Point(e.X,e.Y);   
            for (int i = 0; i < 8; i++)
            {
                _MControl.Parent.Controls.Add(_lbl[i]);
                _lbl[i].BringToFront();
            }
            MoveHandles();
            HideHandles();
        }

        private void CreateTextBox()
        {
            _textbox = new MaskedTextBox();
            _textbox.CreateControl();
            _textbox.Parent = _MControl.Parent;
            _textbox.Width = 0;
            _textbox.Height = 0;
            _textbox.TabStop = true;
            _textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox_KeyDown);
        }

        private void ControlLocality()
        {
            if (_MControl.Location.X < 0)
            {
                _MControl.Visible = false;
                _MControl.Left = 0;
            }
            if (_MControl.Location.Y < 0)
            {
                _MControl.Visible = false;
                _MControl.Top = 0;
            }
            if (_MControl.Location.X + _MControl.Width > _MControl.Parent.Width)
            {
                _MControl.Visible = false;
                _MControl.Left = _MControl.Parent.Width - _MControl.Width;
            }
            if (_MControl.Location.Y + _MControl.Height > _MControl.Parent.Height)
            {
                _MControl.Visible = false;
                _MControl.Top = _MControl.Parent.Height - _MControl.Height;
            }
        }

        private void HideHandles()
        {
            for (int i = 0; i < 8; i++)
            {
                _lbl[i].Visible = false;
            }
        }

        private void MoveHandles()
        {
            int sX = _MControl.Left - BOX_SIZE;
            int sY = _MControl.Top - BOX_SIZE;
            int sW = _MControl.Width + BOX_SIZE;
            int sH = _MControl.Height + BOX_SIZE;
            int hB = BOX_SIZE / 2;
            int[] arrPosX = new int[] {sX+hB, sX + sW / 2, sX + sW-hB, sX + sW-hB,
                                          sX + sW-hB, sX + sW / 2, sX+hB, sX+hB};
            int[] arrPosY = new int[] {sY+hB, sY+hB, sY+hB, sY + sH / 2, sY + sH-hB,
                                          sY + sH-hB, sY + sH-hB, sY + sH / 2};
            for (int i = 0; i < 8; i++)
            {
                _lbl[i].SetBounds(arrPosX[i], arrPosY[i], BOX_SIZE, BOX_SIZE);
            }
        }

        private void ShowHandles()
        {
            if (_MControl != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    _lbl[i].Visible = true;
                }
            }
        }

        #endregion
    }
}
