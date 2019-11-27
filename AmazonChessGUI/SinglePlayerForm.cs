using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }
}
