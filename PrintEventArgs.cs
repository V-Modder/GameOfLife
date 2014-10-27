using System;
using System.Runtime.CompilerServices;

namespace GameOfLife
{
    public class PrintEventArgs : EventArgs
    {

        public PrintEventArgs(char[,] grid, int RoundCount)
        {
            this.Grid = grid;
            this.RoundCount = RoundCount;
        }

        public char[,] Grid
        {
            get;
            private set;
        }

        public int RoundCount
        {
            get;
            private set;
        }
    }
}
