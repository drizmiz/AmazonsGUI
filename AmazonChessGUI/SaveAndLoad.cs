using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace AmazonChessGUI
{
    static class SaveAMZ
    {
        static public void SaveGame(string filename, ChessGame game)
        {

        }
        static public void SaveStep(ChessGame game)
        {

        }
    }
    static class LoadAMZ
    {
        static public ChessGame LoadGame(string filename)
        {
            ChessGame game = new ChessGame();
            try
            {
                StreamReader reader = new StreamReader(filename);
                int turn_cnt = Convert.ToInt32(reader.ReadLine());
                Stack<string> remainders = new Stack<string>();
                for (; !reader.EndOfStream;)
                {
                    string remainder = reader.ReadLine();
                    remainders.Push(remainder);
                }

                game.Text = reader.ReadToEnd();
                reader.Close();
                // stream.Close();
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show("文件不存在，是否重试？", "文件不存在",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                    == DialogResult.Cancel)
                    return null;
                else
                    return LoadGame(filename);
            }
            catch (Exception)
            {
                MessageBox.Show("读文件失败！", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return game;
        }
        static public void LoadStep(ref ChessGame game)
        {

        }
    }

    public struct Chess
    {
        public int nx;
        public int ny;
    }

    public struct Move
    {
        public Chess source;
        public Chess dest;
        public Chess arrow;
    }

    public class ChessGame
    {
        public Color Color { get; set; }
        public string Text { get; set; }

        public ChessGame() { Text = ""; }

        public void MakeMove(Move move)
        {
            Text += move.source.nx + " ";
            Text += move.source.ny + " ";
            Text += move.dest.nx + " ";
            Text += move.dest.ny + " ";
            Text += move.arrow.nx + " ";
            Text += move.arrow.ny;
            Text += Environment.NewLine;
        }
        public Move[] GetMoves()
        {
            string []splits = new string[1];
            splits[0] = Environment.NewLine;
            string[] vs = Text.Split(splits,StringSplitOptions.RemoveEmptyEntries);

            Move[] ret = new Move[vs.Length];

            string str;
            for (int i = 0; i < vs.Length; ++i)
            {
                str = vs[i];
                string[] coordinates = str.Split(' ');
                if (coordinates.Length != 6)
                    throw new ArgumentException("invalid text");
                ret[i].source.nx = Convert.ToInt32(coordinates[0]);
                ret[i].source.ny = Convert.ToInt32(coordinates[1]);
                ret[i].dest.nx = Convert.ToInt32(coordinates[2]);
                ret[i].dest.ny = Convert.ToInt32(coordinates[3]);
                ret[i].arrow.nx = Convert.ToInt32(coordinates[4]);
                ret[i].arrow.ny = Convert.ToInt32(coordinates[5]);
            }

            return ret;
        }

        public bool AutoMoveNext(SinglePlayerForm form)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = "amazons_recover.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };
                Process processer = new Process
                {
                    StartInfo = info
                };
                processer.Start();
                var cout = processer.StandardOutput;
                var cin = processer.StandardInput;

                int totalMoveCount = Text.Split('\n').Length - 1;
                int turnCount = totalMoveCount / 2 + 1;
                cin.Write(turnCount + Environment.NewLine);
                if (totalMoveCount % 2 == 0)
                    cin.Write("-1 -1 -1 -1 -1 -1" + Environment.NewLine);
                cin.Write(Text);

                form.waitingLabel.Visible = true;
                Thread.Sleep(1100);
                form.waitingLabel.Visible = false;

                string nextMove = cout.ReadLine();
                Text += nextMove.Trim(null) + Environment.NewLine;

                if (!processer.HasExited)
                    processer.Kill();
                processer.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
