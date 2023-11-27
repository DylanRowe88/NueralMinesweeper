﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NueralMinesweeper
{
    public class Minefield : IComparable // Class to keep track of edges with consolidated methods
    {
        private class Mine
        {
            public Mine() { mineVal = 0; X = -1; Y = -1; } // Null Node
            public Mine(int newMineVal, int newX, int newY)
            {
                mineVal = newMineVal; X = newX; Y = newY;
            }

            public bool isMine;
            public bool isCovered;
            public int mineVal;
            public double X, Y;
            public override string ToString() => $"({X}, {Y})";
        }

        public readonly int width, height;
        public readonly int mineCount;    // How many mines are in field
        public readonly int uncoveredCount; // How many tiles have been uncovered
        public readonly int moveCount;    // How many moves have been made on field
        public readonly int boomCount;    // How many mines have been hit

        private List<Mine> field = new();
        private Random rng = new Random();

        public Minefield(int width, int height, List<int> Minefield) // Input a pre-generated minefield
        {
            foreach (int val in Minefield)
            {
                int index = field.Count;
                int x = index % width;
                int y = index / height;
                field.Add(new(val, x, y));
            }
        }
        public Minefield(int width, int height, int minecount) // Create minefield with dimensions and minecount
        {
            for (int i = 0; i < width * height; i++)
            {
                int index = field.Count;
                int x = index % width;
                int y = index / height;
                // Generate a random number between -2 and 8
                field.Add(new(rng.Next(-2, 9), x, y));
            }
        }
        public int count => field.Count;
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

    }
    // internal class Minesweeper // This was here at first, idk what internal does
    class Minesweeper
    {
        public Minefield Field;
        public Minesweeper(int width, int height, int mineCount) { Field = new(width, height, mineCount); }

        //public readonly List<List<Minefield>> Generations = new();
        //public readonly List<Double> BestFitperGeneration = new();

    }
}