﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace AmazonChessGUI
{
    public partial class LoadPrev : Form
    {
        public LoadPrev()
        {
            InitializeComponent();
        }

        #region 响应鼠标拖拽

        // 这段代码为什么能工作？这我就不知道了，是从Github上下载的，反正能用就行了（x

        const int CURSOR_HTLEFT = 10;
        const int CURSOR_HTRIGHT = 11;
        const int CURSOR_HTTOP = 12;
        const int CURSOR_HTTOPLEFT = 13;
        const int CURSOR_HTTOPRIGHT = 14;
        const int CURSOR_HTBOTTOM = 15;
        const int CURSOR_HTBOTTOMLEFT = 16;
        const int CURSOR_HTBOTTOMRIGHT = 17;

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case 0x0084:
                    base.WndProc(ref msg);
                    Point vPoint = new Point((int)msg.LParam & 0xFFFF,
                        (int)msg.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            msg.Result = (IntPtr)CURSOR_HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            msg.Result = (IntPtr)CURSOR_HTBOTTOMLEFT;
                        else msg.Result = (IntPtr)CURSOR_HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            msg.Result = (IntPtr)CURSOR_HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            msg.Result = (IntPtr)CURSOR_HTBOTTOMRIGHT;
                        else msg.Result = (IntPtr)CURSOR_HTRIGHT;
                    else if (vPoint.Y <= 5)
                        msg.Result = (IntPtr)CURSOR_HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        msg.Result = (IntPtr)CURSOR_HTBOTTOM;
                    break;
                case 0x0201:
                    msg.Msg = 0x00A1;
                    msg.LParam = IntPtr.Zero;
                    msg.WParam = new IntPtr(2);
                    base.WndProc(ref msg);
                    break;
                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        #endregion

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string path;
            if (MessageBox.Show("是否进行快速载入？", "提示",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                path = Environment.CurrentDirectory + "\\prev.amz";
            else
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    DefaultExt = ".amz"
                };
                dialog.ShowDialog();
                path = dialog.FileName;
                dialog.Dispose();
            }
            ChessGame game = LoadAMZ.LoadGame(path);

            new Thread((ThreadStart)delegate
            {
                Application.Run(new SinglePlayerForm(game));
            }).Start();
            Close();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            new Thread((ThreadStart)delegate
            {
                Application.Run(new SinglePlayerForm(new ChessGame()));
            }).Start();
            Close();
        }
    }
}
