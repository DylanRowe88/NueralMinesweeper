using System.Reflection.Metadata.Ecma335;

namespace NueralMinesweeper
{
    public class Minefield : IComparable // Class to keep track of edges with consolidated methods
    {
        private class Tile
        {
            public bool isMine = false;
            public bool isCovered = true;
            public bool isFlagged = false;
            public int adjMineCnt = 0;
            public double Row, Col, Index;
            public Tile(int row, int col, int index)
            {
                Row = row;
                Col = col;
                Index = index;
            }
            public Tile(int newTileVal, int row, int col, int index, bool isMine) // If tileVal is known ahead of time
            {
                adjMineCnt = newTileVal;
                Row = row;
                Col = col;
                Index = index;
                this.isMine = isMine;
            }

            public override string ToString() => $"({Row}, {Col})";
        }

        public readonly int width, height;
        public readonly int fieldSize;    // Width * Height of field
        public readonly int mineCount;    // How many mines are in field
        public readonly int uncoveredCount; // How many tiles have been uncovered
        public readonly int moveCount;    // How many moves have been made on field
        public readonly int boomCount;    // How many mines have been hit

        private bool gameStarted = false;

        private List<Tile> field = new();
        private static Random rng = new Random();
        public List<Delegate> setTileValDel = new List<Delegate>();


        public int GetAdjCount(int index)
        {
            if (field[index].isMine)
                return -1;
            return field[index].adjMineCnt;
        }

        /*public Minefield(int newWidth, int newHeight, List<int> Minefield) // Input a pre-generated minefield
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
        }*/

        public Minefield(int newWidth, int newHeight, int mineCount) // Create minefield with dimensions and minecount
        {
            width = newWidth;
            height = newHeight;
            fieldSize = newWidth * newHeight;
            for (int i = 0; i < width * height; i++)
            {
                int x = field.Count % width;
                int y = field.Count / height;
                field.Add(new Tile(x, y, getIndex(x, y)));
            }
            if (fieldSize != field.Count)
            {
                throw new Exception("Count != fieldSize");
            }
            this.mineCount = mineCount;
        }
        public bool makeMove(int tileIndex)
        {
            if (gameStarted)
            {
                if (!field[tileIndex].isCovered || field[tileIndex].isFlagged) // Already uncovered?
                    return false;
                

                field[tileIndex].isCovered = false;
                setTileValDel[tileIndex].DynamicInvoke(GetAdjCount(tileIndex));
                if (field[tileIndex].adjMineCnt == 0 && !field[tileIndex].isMine)
                {
                    (int, int) RowCol = getRowCol(tileIndex);
                    for (int j = -1; j <= 1; j++)
                        for (int k = -1; k <= 1; k++)
                            if (j == 0 && k == 0)
                                continue;
                            else
                                tryClearing(RowCol.Item1 + j, RowCol.Item2 + k); //all
                }
                return true;

            }
            else // Need to generate mines
            {
                for (int i = 0; i < mineCount; i++)
                {
                    int randMineIndex = rng.Next(fieldSize);
                    while (field[randMineIndex].isMine == true // Keep searching for a tile that isnt already a mine
                        || randMineIndex == tileIndex) // Ensure the first move isn't a mine
                    {
                        randMineIndex = rng.Next(fieldSize);
                    }
                    field[randMineIndex].isMine = true;
                    (int, int) RowCol = getRowCol(randMineIndex);

                    for (int j = -1; j <= 1; j++)
                        for (int k = -1; k <= 1; k++)
                            if (j == 0 && k == 0) 
                                continue;
                            else
                                tryIncrement(RowCol.Item1 + j, RowCol.Item2+k);     // all
                }
                gameStarted = true;
                field[tileIndex].isCovered = false;
                if (field[tileIndex].adjMineCnt == 0)
                {
                    (int, int) RowCol = getRowCol(tileIndex);
                    for (int j = -1; j <= 1; j++)
                        for (int k = -1; k <= 1; k++)
                            if (j == 0 && k == 0)
                                continue;
                            else
                                tryClearing(RowCol.Item1 + j, RowCol.Item2 + k); //all
                }
                setTileValDel[tileIndex].DynamicInvoke(GetAdjCount(tileIndex));
                return true;
            }
        }
        private void tryClearing(int Row, int Col, bool C = false)
        {
            if ((Row <= height && Col <= height && Row >= 0 && Col >= 0) && (getIndex(Row, Col) >= 0 && getIndex(Row, Col) < fieldSize && !field[getIndex(Row, Col)].isMine))
                makeMove(getIndex(Row, Col));
        }
        private void tryIncrement(int Row, int Col)
        {
            if ((Row <= height && Col <= height && Row >= 0 && Col >= 0) && (getIndex(Row, Col) >= 0 && getIndex(Row, Col) < fieldSize && !field[getIndex(Row, Col)].isMine))
                field[getIndex(Row, Col)].adjMineCnt++;   
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

        public bool toggleTileFlag(int tileIndex) => (field[tileIndex].isCovered) ? field[tileIndex].isFlagged = !field[tileIndex].isFlagged : false;

        public double RatioUncovered() => uncoveredCount / field.Count;


        public (int, int) getRowCol(int index) => (index / width, index % width);
        public int getIndex(int row, int col) => (row > height - 1 || col > width - 1 || row < 0 || col < 0 || row * col >= fieldSize) ? -1 : row * width + col;
        
    }

}