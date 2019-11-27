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
        private ChessGame Game { get; set; }

        public ChessTable(ChessGame game)
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            this.Resize += ChessTable_Resize;
            this.MouseClick += ChessTable_MouseClick;
            this.Load += ChessTable_Load;

            Game = game;
            selected.nx = -1;
            selected.ny = -1;

            InitMatrix();
        }

        private void ChessTable_Load(object sender, EventArgs e)
        {
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
        }

        /// <summary>
        /// 半径的平方值，用来确定是否容忍这次放置
        /// </summary>
        private float _RadiusSquare = 0;
        /// <summary>
        /// 计步器，用来确定黑白
        /// </summary>
        private int _Step = 1;

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

        [Description("线的颜色")]
        public Color ColorOfLine = Color.Black;

        protected override void OnPaint(PaintEventArgs pe)
        {
            float tile_width = (float)(1.0 * this.Width / this._vertical);
            float tile_height = (float)(1.0 * this.Height / this._horizontal);

            _RadiusSquare = (tile_width * tile_width + tile_height * tile_height) / 9;

            pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;

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

            for (int row = 0; row < this._horizontal - 1; row++)
            {
                for (int col = 0; col < this._vertical - 1; col++)
                {
                    ChessPiece piec = _Matrix[row * (this._vertical - 1) + col];

                    if (piec.IsOk)
                    {
                        using (SolidBrush solidBrush = new SolidBrush(piec.Color))
                        {
                            float x = (float)((col + 0.5 + 0.2) * tile_width);
                            float y = (float)((row + 0.5 + 0.2) * tile_height);
                            float width = (float)(tile_width * 0.6);
                            float height = (float)(tile_height * 0.6);

                            pe.Graphics.FillEllipse(solidBrush, x, y, width, height);
                            base.OnPaint(pe);
                        }
                    }
                }
            }
        }
        ref ChessPiece Piece(int nx, int ny)
        {
            return ref _Matrix[nx + ny * (_vertical - 1)];
        }
        void PaintForXY(int nx, int ny, Color color)
        {
            // check
            if (nx < 0 || ny < 0 || nx >= _horizontal - 1 || ny >= _vertical - 1)
                return;

            ChessPiece piec = _Matrix[nx + ny * (_vertical - 1)];
            // if (!piec.IsOk)
            // {
            piec.Step = _Step;
            piec.Color = color;
            piec.IsOk = true;

            Invalidate();
            // }
        }

        Chess selected;
        bool validSelect = false;

        enum WhoseMove
        {
            blackMove = 0,
            blackArrow = 1,
            whiteMove = 2,
            whiteArrow = 3,
            invalid = 4
        }

        WhoseMove currentMove = WhoseMove.blackMove;

        void NextMove(ref WhoseMove move)
        {
            move = (++move == WhoseMove.invalid) ? WhoseMove.blackMove : move;
        }

        void PrevMove(ref WhoseMove move)
        {
            move = (move == WhoseMove.blackMove) ? WhoseMove.whiteArrow : (--move);
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

                    if (div_width * div_width + div_height * div_height <= this._RadiusSquare)
                    {
                        if (currentMove == WhoseMove.blackMove || currentMove == WhoseMove.whiteMove)   // 该移动了
                        {
                            if (!validSelect) // 未选中
                            {
                                ChessPiece piece = Piece(nx, ny);
                                if (piece.IsOk)
                                {
                                    if ((piece.Color == Color.Black && currentMove == WhoseMove.blackMove) ||
                                        (piece.Color == Color.White && currentMove == WhoseMove.whiteMove))
                                    {
                                        selected.nx = nx;
                                        selected.ny = ny;
                                        piece.Color = Color.Yellow;
                                        validSelect = true;
                                        // piec.IsOk = true;

                                        Invalidate();
                                    }
                                }
                            }
                            else //已选中
                            {
                                ChessPiece piece = Piece(selected.nx, selected.ny);
                                piece.Step = 0;
                                piece.IsOk = false;

                                _Step++;

                                if (currentMove == WhoseMove.blackMove)
                                    PaintForXY(nx, ny, Color.Black);
                                else
                                    PaintForXY(nx, ny, Color.White);

                                NextMove(ref currentMove);
                                validSelect = false;
                            }
                        }
                        else        // 该放障碍物了
                        {
                            ChessPiece piece = Piece(nx, ny);
                            if (!piece.IsOk)
                            {
                                PaintForXY(nx, ny, Color.Red);
                                NextMove(ref currentMove);
                            }
                        }
                    }

                    break;

                case MouseButtons.Right:
                    // right -> undo

                    for (int i = 0; i < _Matrix.Length; i++)
                    {
                        if (_Matrix[i].IsOk && _Matrix[i].Step == _Step)
                        {
                            _Step--;
                            _Matrix[i].Step = 0;
                            _Matrix[i].IsOk = false;

                            Invalidate();

                            break;
                        }
                    }

                    break;
            }
        }

        void ChessTable_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        ChessPiece[] _Matrix;

        private void InitMatrix()
        {
            _Matrix = new ChessPiece[(_horizontal - 1) * (_vertical - 1)];

            for (int i = 0; i < _Matrix.Length; i++)
            {
                _Matrix[i] = new ChessPiece
                {
                    Color = Color.Black,
                    IsOk = false,
                    Step = 0
                };
            }
        }
    }

    public class ChessPiece
    {
        public Color Color { set; get; }
        public bool IsOk { set; get; }
        public int Step { set; get; }
    }
}
