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
                if (newIndex == -1)
                {
                }
                Row = newRow; Col = newCol; Index = newIndex;
            }
            public Tile(int newTileVal, int newRow, int newCol, int newIndex, bool newIsMine) // If tileVal is known ahead of time
            {
                if (newIndex == -1)
                {
                }
                adjMineCnt = newTileVal; Row = newRow; Col = newCol; Index = newIndex; isMine = newIsMine;
            }
            public bool isMine = false;
            public bool isCovered = true;
            public bool isFlagged = false;
            public int adjMineCnt = 0;
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
                if (field[index].isCovered) { return 0; }
                if (field[index].isMine) { return -1; }
                return field[index].adjMineCnt;
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
                field.Add(new(val, x, y, index, Minefield[index] == -1 ? true:false));
                if(val == -1){mineCount++; }
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
                field.Add(new(x, y, getIndex(x, y)));
            }
            fieldSize = field.Count;
            mineCount = newMinecount;
            for (int i = 0; i < mineCount; i++)
            {
                int randMineIndex = rng.Next(fieldSize);
                while (field[randMineIndex].isMine == true)
                {
                    randMineIndex = rng.Next(fieldSize);
                }
                field[randMineIndex].isMine = true;
                (int, int) RowCol = getRowCol(randMineIndex);

                tryIncrement(RowCol.Item1 + 1, RowCol.Item2);     // Right
                tryIncrement(RowCol.Item1 + 1, RowCol.Item2 + 1); // Down/Right
                tryIncrement(RowCol.Item1,     RowCol.Item2 + 1); // Down

                tryIncrement(RowCol.Item1 - 1, RowCol.Item2);     // Left
                tryIncrement(RowCol.Item1 - 1, RowCol.Item2 - 1); // Up/Left
                tryIncrement(RowCol.Item1,     RowCol.Item2 - 1); // Up

                tryIncrement(RowCol.Item1 + 1, RowCol.Item2 - 1); // Up/Right
                tryIncrement(RowCol.Item1 - 1, RowCol.Item2 + 1); // Down/Left
            }
        }
        private void tryIncrement(int Row, int Col)
        {
            int index = getIndex(Row, Col);
            if (index >= 0 && index < fieldSize && !field[index].isMine) // Ensure tile is on field and not a mine
            {
                field[index].adjMineCnt++;
                if (Row == 0)
                {
                }
            }
        }
        public void makeMove(int mineIndex)
        {
            if (!field[mineIndex].isCovered) // Already uncovered?
            {
                return;
            }
            // It is a covered tile
            if (!field[mineIndex].isFlagged) // Flagged? (Do nothing)
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
        public int getIndex(int row, int col)
        {
            if (row < 0 || col < 0 || row * col >= fieldSize) { return -1; }
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