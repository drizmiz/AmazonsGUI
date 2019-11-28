using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AmazonChessGUI
{
    public partial class SinglePlayerForm : Form
    {
        public ChessGame Game { get; set; }

        public SinglePlayerForm(ChessGame game)
        {
            Game = game;
            InitializeComponent();

            chessTable.InitGame(game, this);
        }

        private void SinglePlayerForm_Load(object sender, EventArgs e)
        {
            waitingLabel.Visible = false;
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

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            var form = new HelpForm();
            form.Show();
        }

        private void RegretOneStep()
        {
            if (Game.Text == "") return;

            string[] moves = Game.Text.Trim().Split('\n');
            string lastMove = moves.Last();
            Game.Text = "";
            for (int i = 0; i < moves.Length - 1; ++i)
                Game.Text += moves[i] + Environment.NewLine;
            string[] coordinates = lastMove.Trim().Split(' ');

            if (coordinates.Length == 4 || coordinates.Length == 6)
            {
                ChessPiece pieceSource = chessTable.Piece(
                    Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]));
                ChessPiece pieceDest = chessTable.Piece(
                    Convert.ToInt32(coordinates[2]), Convert.ToInt32(coordinates[3]));

                if (coordinates.Length == 6)
                {
                    ChessPiece pieceArrow = chessTable.Piece(
                        Convert.ToInt32(coordinates[4]), Convert.ToInt32(coordinates[5]));
                    pieceArrow.Color = Color.Empty;
                    pieceArrow.IsOk = false;

                    ChessTable.PrevMove(ref chessTable.currentMove);
                }

                pieceSource.Color = pieceDest.Color;
                pieceSource.IsOk = true;

                pieceDest.Color = Color.Empty;
                pieceDest.IsOk = false;

                ChessTable.PrevMove(ref chessTable.currentMove);
            }
            else throw new Exception("internal error");

            chessTable.UnselectSelected();
            chessTable.Invalidate();
        }

        private void RegretButton_Click(object sender, EventArgs e)
        {
            if (chessTable.currentMove == ChessTable.WhoseMove.blackMove ||
                chessTable.currentMove == ChessTable.WhoseMove.whiteMove)
                RegretOneStep();
            RegretOneStep();
        }

        private void HintButton_Click(object sender, EventArgs e)
        {
            
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string path;
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "亚马逊棋存档文件(*.amz)|*.amz|" +
                    "文本文件(*.txt)|*.txt" +
                    "|所有文件(*.*)|*.*",
                FileName = "quick_save.amz",
                InitialDirectory = Environment.CurrentDirectory,
                DefaultExt = "amz"
            })
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                path = dialog.FileName;
            }
            if (path == "") return;
            ChessGame game = LoadAMZ.LoadGame(path);
            if (game == null) return;
            var newTrd = new Thread((ThreadStart)delegate
             {
                 Application.Run(new SinglePlayerForm(game));
             });
            newTrd.SetApartmentState(ApartmentState.STA);
            newTrd.Start();
            Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if(!(chessTable.currentMove == ChessTable.WhoseMove.blackMove||
                chessTable.currentMove == ChessTable.WhoseMove.whiteMove))
            {
                MessageBox.Show("请在完成当前移动（放置Arrow）后再保存~", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string path;
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "亚马逊棋存档文件(*.amz)|*.amz|" +
                "文本文件(*.txt)|*.txt" +
                "|所有文件(*.*)|*.*",
                FileName = "quick_save.amz",
                InitialDirectory = Environment.CurrentDirectory,
                DefaultExt = "amz"
            })
            {
                dialog.ShowDialog();
                path = dialog.FileName;
            }
            if (path == "") return;
            SaveAMZ.SaveGame(path, Game);
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            var newTrd = new Thread((ThreadStart)delegate
            {
                Application.Run(new SinglePlayerForm(new ChessGame()));
            });
            newTrd.SetApartmentState(ApartmentState.STA);
            newTrd.Start();
            Close();
        }

    }
}
