using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmazonChessGUI
{
    public partial class ChessTable : UserControl
    {
        private SinglePlayerForm spf;

        private ChessGame Game { get; set; }

        public ChessTable()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            this.Resize += ChessTable_Resize;
            this.MouseClick += ChessTable_MouseClick;
            this.Load += ChessTable_Load;
        }

        public void InitGame(ChessGame game,SinglePlayerForm form)
        {
            Game = game;
            spf = form;

            selected.nx = -1;
            selected.ny = -1;

            InitMatrix();
            
            foreach(Move move in Game.GetMoves())
            {
                MovePaint(move);
            }

            Invalidate();
        }

        private void ChessTable_Load(object sender, EventArgs e)
        {
            // if (!inited) throw new Exception();

            Color color = Color.Black;
            PaintForXY(0, 2, color);
            PaintForXY(2, 0, color);
            PaintForXY(5, 0, color);
            PaintForXY(7, 2, color);
            color = Color.White;
            PaintForXY(0, 5, color);
            PaintForXY(2, 7, color);
            PaintForXY(5, 7, color);
            PaintForXY(7, 5, color);

            //foreach(var move in Game.GetMoves())
            //{
            //    MovePaint(move);
            //}
        }

        /// <summary>
        /// 半径的平方值，用来确定是否容忍这次放置
        /// </summary>
        private float _RadiusSquare = 0;

        /// <summary>
        /// 纵线数
        /// </summary>
        private int _vertical = 9;
        [Description("纵线数")]
        public int Vertical
        {
            get { return _vertical; }
            set
            {
                _vertical = value;
                InitMatrix();
                Invalidate();
            }
        }

        /// <summary>
        /// 横线数
        /// </summary>
        private int _horizontal = 9;
        [Description("横线数")]
        public int Horizontal
        {
            get { return _horizontal; }
            set
            {
                _horizontal = value;
                InitMatrix();
                Invalidate();
            }
        }

        public int Col { get { return _vertical - 1; } }
        public int Row { get { return _horizontal - 1; } }

        [Description("线的颜色")]
        public Color ColorOfLine = Color.Black;

        protected override void OnPaint(PaintEventArgs pe)
        {
            float tile_width = (float)(1.0 * this.Width / this._vertical);
            float tile_height = (float)(1.0 * this.Height / this._horizontal);

            _RadiusSquare = (tile_width * tile_width + tile_height * tile_height) / 9;

            pe.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            using (Pen pen = new Pen(new SolidBrush(ColorOfLine), 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                for (int row = 1; row <= _horizontal; row++)
                {
                    pe.Graphics.DrawLine(pen, (float)(tile_width * 0.5), (float)(tile_height * (row - 0.5)), (float)(tile_width * (this._vertical - 0.5)), (float)(tile_height * (row - 0.5)));
                    base.OnPaint(pe);
                }

                for (int col = 1; col <= _vertical; col++)
                {
                    pe.Graphics.DrawLine(pen, (float)(tile_width * (col - 0.5)), (float)(tile_height * 0.5), (float)(tile_width * (col - 0.5)), (float)(tile_height * (this._horizontal - 0.5)));
                    base.OnPaint(pe);
                }
            }

            pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            for (int row = 0; row < this._horizontal - 1; row++)
            {
                for (int col = 0; col < this._vertical - 1; col++)
                {
                    ChessPiece piec = _Matrix[row, col];

                    if (piec.IsOk)
                    {
                        using (Pen pen = new Pen(Color.Black,(float)4.0))
                        {
                            using (SolidBrush solidBrush = new SolidBrush(piec.Color))
                            {
                                float x = (float)((col + 0.5 + 0.2) * tile_width);
                                float y = (float)((row + 0.5 + 0.2) * tile_height);
                                float width = (float)(tile_width * 0.6);
                                float height = (float)(tile_height * 0.6);

                                pe.Graphics.DrawEllipse(pen, x, y, width, height);
                                pe.Graphics.FillEllipse(solidBrush, x, y, width, height);
                                base.OnPaint(pe);
                            }
                        }
                    }
                }
            }
        }

        public ref ChessPiece Piece(int nx, int ny)
        {
            return ref _Matrix[ny, nx];
        }
        ref ChessPiece Piece(Chess chess)
        {
            return ref _Matrix[chess.ny, chess.nx];
        }
        void PaintForXY(int nx, int ny, Color color)
        {
            // check
            if (nx < 0 || ny < 0 || nx >= _horizontal - 1 || ny >= _vertical - 1)
                return;

            ChessPiece piece = _Matrix[ny, nx];
            // if (!piec.IsOk)
            // {
            piece.Color = color;
            piece.IsOk = true;

            Invalidate();
            // }
        }

        Chess selected;
        Chess lastplace;
        bool validSelect = false;

        public enum WhoseMove
        {
            blackMove = 0,
            blackArrow = 1,
            whiteMove = 2,
            whiteArrow = 3,
            invalid = 4
        }

        public WhoseMove currentMove = WhoseMove.blackMove;

        static public void NextMove(ref WhoseMove move)
        {
            move = (++move == WhoseMove.invalid) ? WhoseMove.blackMove : move;
        }

        static public void PrevMove(ref WhoseMove move)
        {
            move = (move == WhoseMove.blackMove) ? WhoseMove.whiteArrow : (--move);
        }

        bool ValidMove(int prev_nx, int prev_ny, int nx, int ny)
        {
            int[] moveDirection = { -1, 1, -8, 8, -7, 7, -9, 9 };
            foreach (var direction in moveDirection)
            {
                for (int step = 1; ; ++step)
                {
                    int idx = prev_nx * 8 + prev_ny + step * direction;
                    int x = idx / 8; int y = idx % 8;
                    if (x < 0 || x >= 8 || y < 0 || y >= 8 || Piece(x, y).IsOk)
                        break;
                    if (x == nx && y == ny)
                        return true;
                }
            }
            return false;
        }

        public void UnselectSelected()
        {
            if (currentMove == WhoseMove.blackMove || currentMove == WhoseMove.whiteMove)   // 该移动了
            {
                if (validSelect)
                {
                    ChessPiece piece = Piece(selected.nx, selected.ny);
                    if (currentMove == WhoseMove.blackMove)
                        piece.Color = Color.Black;
                    else if (currentMove == WhoseMove.whiteMove)
                        piece.Color = Color.White;
                    validSelect = false;

                    Invalidate();
                }
            }
        }

        void ChessTable_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // left -> place a chess

                    float tile_width = (float)(1.0 * this.Width / this._vertical);
                    float tile_height = (float)(1.0 * this.Height / this._horizontal);

                    int nx = Convert.ToInt32(Math.Round(e.X / tile_width - 1.0));
                    int ny = Convert.ToInt32(Math.Round(e.Y / tile_height - 1.0));

                    if (nx < 0 || ny < 0 || nx >= _horizontal - 1 || ny >= _vertical - 1)
                        break;

                    float div_width = (float)((nx + 1.0) * tile_width) - e.X;
                    float div_height = (float)((ny + 1.0) * tile_height) - e.Y;

                    if (div_width * div_width + div_height * div_height <= _RadiusSquare)
                    {
                        if (currentMove == WhoseMove.blackMove || currentMove == WhoseMove.whiteMove)   // 该移动了
                        {
                            if (validSelect) //已选中
                            {
                                if (ValidMove(selected.nx, selected.ny, nx, ny))
                                {
                                    if (!(selected.nx == nx && selected.ny == ny))
                                    {
                                        ChessPiece piece = Piece(selected.nx, selected.ny);
                                        piece.IsOk = false;

                                        if (currentMove == WhoseMove.blackMove)
                                            PaintForXY(nx, ny, Color.Black);
                                        else
                                            PaintForXY(nx, ny, Color.White);

                                        Game.Text +=
                                            selected.nx + " " + selected.ny + " "
                                            + nx + " " + ny + " ";

                                        NextMove(ref currentMove);
                                        lastplace.nx = nx;
                                        lastplace.ny = ny;
                                        validSelect = false;
                                    }
                                }
                                else
                                {
                                    ChessPiece piece = Piece(selected.nx, selected.ny);
                                    if (currentMove == WhoseMove.blackMove)
                                        piece.Color = Color.Black;
                                    else if (currentMove == WhoseMove.whiteMove)
                                        piece.Color = Color.White;
                                    validSelect = false;

                                    Invalidate();
                                }
                            }
                            // select
                            {
                                ChessPiece piece = Piece(nx, ny);
                                if (piece.IsOk)
                                {
                                    if (piece.Color == Color.Black && currentMove == WhoseMove.blackMove)
                                    {

                                        piece.Color = Color.DimGray;
                                        selected.nx = nx;
                                        selected.ny = ny;
                                        validSelect = true;

                                        Invalidate();
                                    }
                                    else if (piece.Color == Color.White && currentMove == WhoseMove.whiteMove)
                                    {

                                        piece.Color = Color.LemonChiffon;
                                        selected.nx = nx;
                                        selected.ny = ny;
                                        validSelect = true;

                                        Invalidate();
                                    }
                                }
                            }
                        }
                        else        // 该放障碍物了
                        {
                            ChessPiece piece = Piece(nx, ny);
                            if (!piece.IsOk)
                            {
                                if (ValidMove(lastplace.nx, lastplace.ny, nx, ny))
                                {
                                    PaintForXY(nx, ny, Color.DodgerBlue);
                                    OnPaint(new PaintEventArgs(
                                        CreateGraphics(),
                                        new Rectangle(0, 0, Width, Height)));

                                    Game.Text += nx + " " + ny + Environment.NewLine;

                                    if (!Game.AutoMoveNext(spf))
                                        throw new Exception("bad call");
                                    MovePaint(Game.GetMoves().Last());
                                    NextMove(ref currentMove);
                                    NextMove(ref currentMove);

                                    NextMove(ref currentMove);
                                }
                            }
                        }
                    }

                    break;

                case MouseButtons.Right:
                    // right -> unselect
                    UnselectSelected();
                    break;
            }
        }

        void ChessTable_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        void MovePaint(Move move)
        {
            ChessPiece pieceSource = Piece(move.source);
            ChessPiece pieceDest = Piece(move.dest);
            ChessPiece pieceArrow = Piece(move.arrow);

            pieceDest.Color = pieceSource.Color;
            pieceDest.IsOk = true;

            pieceSource.Color = Color.Empty;
            pieceSource.IsOk = false;

            pieceArrow.Color = Color.DodgerBlue;
            pieceArrow.IsOk = true;

            Invalidate();
        }

        void Flush()
        {

        }

        ChessPiece[,] _Matrix;

        private void InitMatrix()
        {
            _Matrix = new ChessPiece[Row, Col];

            for (int i = 0; i < _Matrix.Length; i++)
            {
                _Matrix[i / Col, i % Col] = new ChessPiece
                {
                    Color = Color.Black,
                    IsOk = false
                };
            }
        }
    }

    public class ChessPiece
    {
        public Color Color { set; get; }
        public bool IsOk { set; get; }
    }
}
