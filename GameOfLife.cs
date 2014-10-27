namespace GameOfLife
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class GameOfLife
    {
        private char alive;
        private bool bStop;
        private BackgroundWorker bw;
        private char dead;
        private char[,] grid;
        private Size gridSize;
        private int roundCount;
        private int speed;

        public event MyPrintHandler PrintGrid;

        public GameOfLife(int x, int y, char alive = 'O', char dead = ' ')
        {
            this.gridSize.Width = x;
            this.gridSize.Height = y;
            this.alive = alive;
            this.dead = dead;
            this.grid = new char[this.gridSize.Width, this.gridSize.Height];
            this.speed = 100;
            this.bStop = false;
            this.roundCount = 0;
            for (int i = 0; i < this.gridSize.Height; i++)
            {
                for (int j = 0; j < this.gridSize.Width; j++)
                {
                    this.grid[j, i] = ' ';
                }
            }
            this.bw = new BackgroundWorker();
            this.bw.DoWork += new DoWorkEventHandler(this.bw_DoWork);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            this.StartLife((char[,]) e.Argument);
        }

        private char CheckCell(int pointX, int pointY)
        {
            int num = this.grid.GetLength(1) - 1;
            int num2 = this.grid.GetLength(0) - 1;
            int num3 = ((pointX - 1) < 0) ? num2 : (pointX - 1);
            int num4 = ((pointX + 1) > num2) ? 0 : (pointX + 1);
            int num5 = ((pointY - 1) < 0) ? num : (pointY - 1);
            int num6 = ((pointY + 1) > num) ? 0 : (pointY + 1);
            int num7 = 0;
            if (this.grid[num3, num5] == this.alive)
            {
                num7++;
            }
            if (this.grid[pointX, num5] == this.alive)
            {
                num7++;
            }
            if (this.grid[num4, num5] == this.alive)
            {
                num7++;
            }
            if (this.grid[num3, pointY] == this.alive)
            {
                num7++;
            }
            if (this.grid[num4, pointY] == this.alive)
            {
                num7++;
            }
            if (this.grid[num3, num6] == this.alive)
            {
                num7++;
            }
            if (this.grid[pointX, num6] == this.alive)
            {
                num7++;
            }
            if (this.grid[num4, num6] == this.alive)
            {
                num7++;
            }
            if (this.grid[pointX, pointY] == this.dead)
            {
                if (num7 == 3)
                {
                    return this.alive;
                }
                return this.dead;
            }
            if ((this.grid[pointX, pointY] != this.alive) || (num7 >= 2))
            {
                if (((this.grid[pointX, pointY] == this.alive) && (num7 >= 2)) && (num7 <= 3))
                {
                    return this.alive;
                }
                if ((this.grid[pointX, pointY] == this.alive) && (num7 > 3))
                {
                    return this.dead;
                }
            }
            return this.dead;
        }

        private void Life()
        {
            while (true)
            {
                while (this.bStop)
                {
                    Thread.Sleep(100);
                }
                char[,] c = new char[this.grid.GetLength(0), this.grid.GetLength(1)];
                for (int i = 0; i < this.grid.GetLength(1); i++)
                {
                    for (int j = 0; j < this.grid.GetLength(0); j++)
                    {
                        c[j, i] = this.CheckCell(j, i);
                    }
                }
                this.Print(c, ++this.roundCount);
                this.grid = c;
                Thread.Sleep(this.speed);
            }
        }

        private void Print(char[,] c, int roundCount)
        {
            try
            {
                if (this.PrintGrid != null)
                {
                    this.PrintGrid(this, new PrintEventArgs(c, roundCount));
                }
            }
            catch
            {
            }
        }

        public void Start(char[,] startGrid)
        {
            this.bw.RunWorkerAsync(startGrid);
        }

        private void StartLife(char[,] startGrid)
        {
            if ((startGrid != null) && (startGrid.GetLength(0) != 0))
            {
                int length;
                int num2;
                if (startGrid.GetLength(0) < this.grid.GetLength(0))
                {
                    length = startGrid.GetLength(0);
                }
                else
                {
                    length = this.grid.GetLength(0);
                }
                if (startGrid.GetLength(1) < this.grid.GetLength(1))
                {
                    num2 = startGrid.GetLength(1);
                }
                else
                {
                    num2 = this.grid.GetLength(1);
                }
                for (int i = 0; i < num2; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        this.grid[j, i] = startGrid[j, i];
                    }
                }
            }
            else
            {
                Random random = new Random();
                for (int k = 0; k < ((this.grid.GetLength(0) * this.grid.GetLength(1)) * 0.33); k++)
                {
                    this.grid[random.Next(0, this.grid.GetLength(0)), random.Next(0, this.grid.GetLength(1))] = this.alive;
                }
            }
            this.Print(null, 0);
            this.Life();
        }

        public char Alive
        {
            get
            {
                return this.alive;
            }
        }

        public char Dead
        {
            get
            {
                return this.dead;
            }
        }

        public char[,] Grid
        {
            get
            {
                return this.grid;
            }
        }

        public bool Pause
        {
            get
            {
                return this.bStop;
            }
            set
            {
                this.bStop = value;
            }
        }

        public int Speed
        {
            get
            {
                return this.speed;
            }
            set
            {
                if ((value > 0) && (value <= 0x3e8))
                {
                    this.speed = value;
                }
            }
        }

        public delegate void MyPrintHandler(object sender, PrintEventArgs e);
    }
}
