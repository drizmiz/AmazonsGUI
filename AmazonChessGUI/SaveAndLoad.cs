using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

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

                // stream.Close();
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show("文件不存在，是否重试？", "文件不存在",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                    == DialogResult.Cancel)
                    if (MessageBox.Show("是否开始新对局？", "新对局",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                       == DialogResult.Yes)
                        return new ChessGame();
                    else
                        throw new Exception("file doesn't exist");
                else 
                {

                }
            }
            catch (Exception)
            {
                throw new Exception("can not read");
            }
            return game;
        }
        static public void LoadStep(ref ChessGame game)
        {

        }
    }

    static class RunProcess
    {
        static public Process StartProcess(out StreamReader cout,out StreamWriter cin, string path, string arg)
        {
            Process process = new Process();   
            ProcessStartInfo startInfo = new ProcessStartInfo(path, arg.Trim());
            process.StartInfo = startInfo;
            process.Start();
            cout = process.StandardOutput;
            cin = process.StandardInput;
            return process;
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
        public string Text { get => text; set => text = value; }

        private string text = "";

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
    }
}
