using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    class Cell : Button
    {
        public int r, c;
        public int MinesCount;
        public bool isOpen;
        public bool isFlagged;

        public Cell(int ri, int ci, int W, int H, int Dimensions)
        {
            r = ri;
            c = ci;
            isOpen = false;
            isFlagged = false;
            MinesCount = 0;
            this.Width = W / Dimensions;
            this.Height = H / Dimensions;
            this.Margin = new Padding(0);
        }
    }
}
