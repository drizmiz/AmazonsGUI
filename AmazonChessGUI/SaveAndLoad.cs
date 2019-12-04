using System;
using System.IO;
using System.Windows.Forms;

namespace AmazonChessGUI
{
    static class SaveAMZ
    {
        static public bool SaveGame(string filename, in ChessGame game)
        {
            try
            {
                StreamWriter reader = new StreamWriter(filename, false);
                reader.Write(game.Text);
                reader.Close();
                // stream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("写文件失败！", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
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
                /*
                Stack<string> remainders = new Stack<string>();
                for (; !reader.EndOfStream;)
                {
                    string remainder = reader.ReadLine();
                    remainders.Push(remainder);
                }
                */
                game.Text = reader.ReadToEnd().Trim() + Environment.NewLine;
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
    }
}
