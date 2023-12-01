using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NueralMinesweeper
{
    public class Minefield : IComparable // Class to keep track of edges with consolidated methods
    {
        private class Tile
        {
            public Tile() { } // Null Tile
            public Tile(int newRow, int newCol, int newIndex)
            {
                Row = newRow; Col = newCol; Index = newIndex;
                if(tileVal == -1)
                {
                    isMine = true;
                }
            }
            public Tile(int newTileVal, int newRow, int newCol, int newIndex) // If tileVal is known ahead of time
            {
                tileVal = newTileVal; Row = newRow; Col = newCol; Index = newIndex;
                if (tileVal == -1)
                {
                    isMine = true;
                }
            }
            public bool isMine = false;
            public bool isCovered = true;
            public bool isFlagged = false;
            public int tileVal = 0;
            public double Row = -1, Col = -1, Index = -1;
            public override string ToString() => $"({Row}, {Col})";
        }

        public readonly int width, height;
        public readonly int fieldSize;    // Width * Height of field
        public readonly int mineCount;    // How many mines are in field
        public readonly int uncoveredCount; // How many tiles have been uncovered
        public readonly int moveCount;    // How many moves have been made on field
        public readonly int boomCount;    // How many mines have been hit

        private List<Tile> field = new();
        private Random rng = new Random();
        public int this[int index]
        {
            get 
            {
                if (field[index].isCovered){return 0;}
                return field[index].tileVal;
            }
        }

        public Minefield(int newWidth, int newHeight, List<int> Minefield) // Input a pre-generated minefield
        {
            width = newWidth; height = newHeight;
            foreach (int val in Minefield)
            {
                int index = field.Count;
                int x = index % width;
                int y = index / height;
                field.Add(new(val, x, y, index));
                if(val == -1){mineCount++;}
            }
            fieldSize = field.Count;
        }

        public Minefield(int newWidth, int newHeight, int newMinecount) // Create minefield with dimensions and minecount
        {
            width = newWidth; height = newHeight;
            for (int i = 0; i < width * height; i++)
            {
                int index = field.Count;
                int x = index % width;
                int y = index / height;
                field.Add(new(x, y, convertRowColtoIndex(x, y, width)));
            }
            fieldSize = field.Count;
            mineCount = newMinecount;
            for (int i = 0; i < mineCount; i++)
            {
                int randMineIndex = rng.Next(fieldSize);
                field[randMineIndex].isMine = true;
                field[randMineIndex].tileVal = -1;
            }
        }
        public void makeMove(int mineIndex)
        {
            if (!field[mineIndex].isCovered)
            {
                return;
            }
            // It is a covered tile
            if (field[mineIndex].tileVal != -1)
            {
                field[mineIndex].isCovered = false;
            }
        }

        public double RatioUncovered()
        {
            return uncoveredCount / field.Count;
        }

        public int CompareTo(object? obj)
        {
            if (obj is Minefield T)
            {
                if (T.RatioUncovered() < RatioUncovered()) { return 1; }
                else if (T.RatioUncovered() > RatioUncovered()) { return -1; }
                else { return 0; }
            }
            return 1;
        }

        public (int, int) getRowCol(int index)
        {
            int row = index / width; // Calculate the row
            int col = index % width; // Calculate the column

            return (row, col);
        }
        static public int convertRowColtoIndex(int row, int col, int width)
        {
            int index = row * width + col; // Calculate index

            return index;
        }
    }
    // internal class Minesweeper // This was here at first, idk what internal does
    class Minesweeper
    {
        public Minefield Field;
        //bool Started = false;

        public Minesweeper(int width, int height, int mineCount)
        {
            Field = new(width, height, mineCount);
        }
    }
}