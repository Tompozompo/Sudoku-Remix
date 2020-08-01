using Sudoku.Puzzles;
using System;
using System.Linq;

namespace SudokuMinimizer
{
    class BasicCmdDisplayStrategy : IDisplayStrategy
    {
        public void DisplayImpl(Puzzle p)
        {
            for (int i = 0; i < p.Size; i++)
            {
                if (i % p.InternalSize == 0)
                {
                    Console.WriteLine("---------------------");
                }
                var row = p.GetRow(i).ToList();
                Console.WriteLine(string.Format("{0,1} {1,1} {2,1} | {3,1} {4,1} {5,1} | {6,1} {7,1} {8,1}", row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8]));
            }
            Console.WriteLine("---------------------");
        }
    }
}
