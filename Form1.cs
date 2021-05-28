using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Minesweeper.Properties;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        int Dimensions = 0;
        int NoOfMines = 0;
        Cell[,] Board;
        int flags = 0;
        int cellr, cellc = 0;
        private int ticks;
        private int score;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ticks++;
            timeLabel.Text = "⏰ Time: " + ticks.ToString();
        }

        private void getNeighborhoodCount()
        {
            for(int r=0;r<Dimensions;r++)
            {
                for(int c=0;c<Dimensions;c++)
                {
                    Board[r, c].Text = Board[r, c].MinesCount.ToString();
                }
            }
        }

        private void PlaceMines()
        {
            for(int i=1;i<=NoOfMines;i++)
            {
                Random rand = new Random();
                int r, c;
                do
                {
                    r = rand.Next(Dimensions);
                    c = rand.Next(Dimensions);
                } 
                while (Board[r, c].MinesCount == -1);
                Board[r, c].MinesCount = -1;

                for(int ri=r-1;ri<=r+1;ri++)
                {
                    if (ri < 0 || ri >= Dimensions)
                        continue;
                    for(int ci=c-1;ci<=c+1;ci++)
                    {
                        if (ci < 0 || ci >= Dimensions)
                            continue;
                        if (Board[ri, ci].MinesCount != -1)
                            Board[ri, ci].MinesCount++;
                    }
                }
            }
        }

        private void OpenSurroundingCells(int r,int c)
        {
            for (int ri = r - 1; ri <= r + 1; ri++)
            {
                if (ri < 0 || ri >= Dimensions)
                    continue;
                for (int ci = c - 1; ci <= c + 1; ci++)
                {
                    if (ri == r && ci == c)
                        continue;
                    if (ci < 0 || ci >= Dimensions)
                        continue;
                    if (Board[ri, ci].MinesCount == -1)
                        continue;
                    if (Board[ri, ci].isFlagged)
                        continue;
                    if (Board[ri, ci].isOpen)
                        continue;
                    else if (Board[ri, ci].MinesCount == 0)
                    {
                        Board[ri, ci].BackColor = Color.Silver;
                        Board[ri, ci].isOpen = true;
                        score++;
                        ScoreLabel.Text = "Score:" + score.ToString();
                    }
                    else
                    {
                        Board[ri, ci].BackColor = Color.Silver;
                        score++;
                        ScoreLabel.Text = "Score:" + score.ToString();
                        Board[ri, ci].Text = Board[ri, ci].MinesCount.ToString();
                        Board[ri, ci].Font = new Font(Board[ri, ci].Font.Name, Board[ri, ci].Font.Size, FontStyle.Bold);
                        if (Board[ri, ci].MinesCount == 1)
                            Board[ri, ci].ForeColor = Color.Blue;
                        else if (Board[ri, ci].MinesCount == 2)
                            Board[ri, ci].ForeColor = Color.Green;
                        else
                            Board[ri, ci].ForeColor = Color.Red;
                        Board[ri, ci].isOpen = true;
                    }
                }
            }
        }

        private void OpenConnectedZeros()
        {
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    if (Board[i, j].isOpen && Board[i, j].MinesCount == 0)
                    {
                        OpenSurroundingCells(i, j);
                        for (int ii = 0; ii < Dimensions; ii++)
                        {
                            for (int jj = 0; jj < Dimensions; jj++)
                            {
                                if (Board[ii, jj].isOpen && Board[ii, jj].MinesCount == 0)
                                    OpenSurroundingCells(ii, jj);
                            }
                        }
                    }
                }
            }
        }

        private bool checkForWin()
        {
            for(int i=0;i<Dimensions;i++)
            {
                for(int j=0;j<Dimensions;j++)
                {
                    if ((!Board[i, j].isOpen) && Board[i, j].MinesCount!=-1)
                    {
                        return false;
                    }
                }
            }
            timer1.Stop();
            return true;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            //Image myImage = Image.FromFile("happy.png");
            ticks = 0;
            score = 0;
            timeLabel.Text = "⏰ Time: " + ticks.ToString();
            ScoreLabel.Text = "Score:" + score.ToString();
            SmileyBox.Text = " 🙂";
            SmileyBox.ForeColor = Color.Red;
            if (Easy.Checked)
            {
                Dimensions = 5;
                NoOfMines = 5;
            }
            else if(Medium.Checked)
            {
                Dimensions = 10;
                NoOfMines = 10;
            }
            else if(Hard.Checked)
            {
                Dimensions = 15;
                NoOfMines = 15;
            }
            else
            {
                MessageBox.Show("Please select a level!");
                return;
            }

            BoardPanel.Controls.Clear();
            Board = new Cell[Dimensions,Dimensions];
            for (int ri=0;ri<Dimensions;ri++)
            {
                for(int ci=0;ci<Dimensions;ci++)
                {
                    Board[ri,ci] = new Cell(ri,ci, BoardPanel.Width, BoardPanel.Height,Dimensions);
                    Board[ri,ci].MouseDown += new MouseEventHandler(this.panel1_MouseDown);
                    BoardPanel.Controls.Add(Board[ri, ci]);
                }
            }
            PlaceMines();
            //getNeighborhoodCount();
            timer1.Start();
        }

        private void performActions(Cell ClickedCell)
        {
            if (ClickedCell.MinesCount == -1)
            {
                ClickedCell.isOpen = true;
                ClickedCell.Text = "💣";
                ClickedCell.BackColor = Color.Red;
                for (int i = 0; i < Dimensions; i++)
                {
                    for (int j = 0; j < Dimensions; j++)
                    {
                        if (Board[i, j].MinesCount == -1)
                        {
                            //Board[i, j].BackColor = Color.Red;
                            Board[i, j].Text= "💣";
                            Board[i, j].Font = new Font(Board[i, j].Font.Name, (14), FontStyle.Bold);

                        }
                        else if (Board[i, j].isFlagged)
                            Board[i, j].Text = "X";
                    }
                }
                timer1.Stop();
                SmileyBox.Text = " 🙁";
                MessageBox.Show("Game Over!");
            }
            else if (ClickedCell.MinesCount == 0)
            {
                ClickedCell.BackColor = Color.Silver;
                score++;
                ScoreLabel.Text = "Score:" + score.ToString();
                ClickedCell.isOpen = true;
                OpenConnectedZeros();
            }
            else
            {
                ClickedCell.BackColor = Color.Silver;
                ClickedCell.isOpen = true;
                ClickedCell.Text = ClickedCell.MinesCount.ToString();
                ClickedCell.Font=new Font(ClickedCell.Font.Name, ClickedCell.Font.Size, FontStyle.Bold);
                if (ClickedCell.MinesCount == 1)
                    ClickedCell.ForeColor = Color.Blue;
                else if (ClickedCell.MinesCount == 2)
                    ClickedCell.ForeColor = Color.Green;
                else
                    ClickedCell.ForeColor = Color.Red;
                score++;
                ScoreLabel.Text = "Score:" + score.ToString();
            }
            if (checkForWin())
            {
                MessageBox.Show("You Win!");
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Cell ClickedCell = (Cell)sender;
            if (ClickedCell.isOpen)
                return;
            switch (e.Button)
            {
                case MouseButtons.Right:
                    if (!ClickedCell.isFlagged)
                    {
                        //ClickedCell.BackColor = Color.Orange;
                        ClickedCell.Text = "🚩";
                        ClickedCell.Font = new Font(ClickedCell.Font.Name, (14), FontStyle.Bold);
                        ClickedCell.ForeColor = Color.Red;
                        ClickedCell.isFlagged = true;
                    }
                    else
                    {
                        ClickedCell.isFlagged = false;
                        ClickedCell.UseVisualStyleBackColor = true;
                        ClickedCell.Text = "";
                        ClickedCell.ForeColor = Color.Black;
                    }
                    break;
                case MouseButtons.Left:
                    {
                        performActions(ClickedCell);
                    }
                    break;
                default:
                    break;
            }   
        }
    }
}
