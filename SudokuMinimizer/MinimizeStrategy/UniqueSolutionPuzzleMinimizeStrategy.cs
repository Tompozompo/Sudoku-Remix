using System;
using System.Collections.Generic;
using System.Linq;

using Sudoku.Cells;
using Sudoku.Puzzles;

namespace SudokuMinimizer
{
    class UniqueSolutionPuzzleMinimizeStrategy : IPuzzleMinimizeStrategy
    {
        public UniqueSolutionPuzzleMinimizeStrategy()
        {
        }

        public Puzzle Minimize(Puzzle puzzle, int minSize)
        {
            if (!RecursiveSolver.UniqueSolution(puzzle)) // if the input doesn't have a unique solution, return null
            {
                return null;
            }

            Puzzle p = puzzle.Clone();
            IList<SudokuCell> allCells = p.GetAllCells().Where(x => x.Value != null).ToList();
            allCells.Shuffle();
            //IList<SudokuCell> sorted = allCells.Where(x => x.Value != null).OrderBy(x => x.AllOptions.Count).ToList();  // Get all valued cells, in option order
            int counter = 0;
            foreach (SudokuCell c in allCells) // Attempt to remove each cell, making sure that a unique solution remains
            {
                if (p.Count <= minSize)
                {
                    break;
                }
                Console.WriteLine(counter + " / " + allCells.Count);
                int val = (int)c.Value;
                c.Value = null;
                //bool unique = p.GetAllCells().Any(x => x.Options.Count == 1) && RecursiveSolver.NoSolution(p, c.Row, c.Col, val);
                bool unique = RecursiveSolver.NoSolution(p, c.Row, c.Col, val);
                if (!unique)
                {
                    c.Value = val;
                }
                counter++;
            }
            return p;
        }
    }
}

