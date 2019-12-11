using System;
using System.Threading;
using System.Windows.Forms;

namespace AmazonChessGUI
{
    public partial class SinglePlayerForm : Form
    {
        private ChessGame Game { get; set; }

        public SinglePlayerForm(ChessGame game)
        {
            Game = game;
            InitializeComponent();
        }

        private void SinglePlayerForm_Load(object sender, EventArgs e)
        {
            waitingLabel.Visible = false;

            chessTable.Init(Game);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            var form = new HelpForm();
            form.Show();
        }

        private void RegretButton_Click(object sender, EventArgs e)
        {
            if (Game.Text == "") return;
            string[] moves = Game.Text.Trim().Split('\n');
            if (moves.Length == 1 && chessTable.CurrentMove == ChessTable.WhoseMove.whiteMove) return;

            chessTable.ValidBoard = true;

            chessTable.UnselectSelected();

            if (chessTable.CurrentMove == ChessTable.WhoseMove.blackMove ||
                chessTable.CurrentMove == ChessTable.WhoseMove.whiteMove)
                chessTable.RegretOneMoveAndPaint();
            chessTable.RegretOneMoveAndPaint();
        }

        private void HintButton_Click(object sender, EventArgs e)
        {
            if (chessTable.CurrentMove != ChessTable.WhoseMove.blackMove &&
                chessTable.CurrentMove != ChessTable.WhoseMove.whiteMove)
            {
                HintMessageShow();
                return;
            }
            chessTable.AutoMoveOnce();
            chessTable.ManuallyRepaint();
            Thread.Sleep(3000);
            chessTable.AutoMoveOnce();
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
            if(!(chessTable.CurrentMove == ChessTable.WhoseMove.blackMove||
                chessTable.CurrentMove == ChessTable.WhoseMove.whiteMove))
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

        private void ReverseButton_Click(object sender, EventArgs e)
        {
            if (chessTable.CurrentMove != ChessTable.WhoseMove.blackMove &&
                chessTable.CurrentMove != ChessTable.WhoseMove.whiteMove)
            {
                HintMessageShow();
                return;
            }
            if(!chessTable.ValidBoard)
            {
                MessageBox.Show("这局游戏已经结束了！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if(!chessTable.AutoMoveOnce())
            {
                chessTable.MoveNext();
                chessTable.MoveNext();
            }
        }

        private void HintMessageShow()
        {
            MessageBox.Show("此操作仅当你还没有做本回合的移动时有效" + Environment.NewLine + "可以单击“悔棋”按钮来撤回这一移动", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
