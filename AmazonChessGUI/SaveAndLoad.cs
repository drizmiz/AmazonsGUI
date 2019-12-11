using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;


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

        static public bool SaveBoardImage(string filename, in ChessTable table)
        {
            try
            {
                Point pt = table.PointToScreen(table.Location);
                var tableX = pt.X - (int)(0.4 * table.TileWidth);
                var tableY = pt.Y - (int)(0.4 * table.TileHeight);
                var tableW = table.Width;
                var tableH = table.Height;
                var newtrd = new Thread((ThreadStart)delegate
                {
                    Thread.Sleep(100);
                    var img = GetScreen(tableX, tableY, tableW, tableH);
                    SaveToFile(img, filename, GetFormat(filename));
                });
                newtrd.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("写文件失败！", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        static private void SaveToFile(Image pic, string filename, ImageFormat format)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            if (format is null) format = GetFormat(filename);

            if (format == ImageFormat.MemoryBmp) pic.Save(filename);
            else pic.Save(filename, format);
        }

        static private ImageFormat GetFormat(string filename)
        {
            ImageFormat format = ImageFormat.MemoryBmp;
            string ext = Path.GetExtension(filename).ToLower();

            if (ext.Equals(".png")) format = ImageFormat.Png;
            else if (ext.Equals(".jpg") || ext.Equals(".jpeg")) format = ImageFormat.Jpeg;
            else if (ext.Equals(".bmp")) format = ImageFormat.Bmp;
            else if (ext.Equals(".gif")) format = ImageFormat.Gif;
            else if (ext.Equals(".ico")) format = ImageFormat.Icon;
            else if (ext.Equals(".emf")) format = ImageFormat.Emf;
            else if (ext.Equals(".exif")) format = ImageFormat.Exif;
            else if (ext.Equals(".tiff")) format = ImageFormat.Tiff;
            else if (ext.Equals(".wmf")) format = ImageFormat.Wmf;
            else if (ext.Equals(".memorybmp")) format = ImageFormat.MemoryBmp;

            return format;
        }

        static private Image GetScreen(int x, int y, int width, int height)
        {
            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(x, y, 0, 0, new Size(width, height));

            return bm;
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
                if (game.Text == Environment.NewLine) game.Text = "";
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
