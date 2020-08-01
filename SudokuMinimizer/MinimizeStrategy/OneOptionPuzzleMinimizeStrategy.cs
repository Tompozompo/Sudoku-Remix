using System.Collections.Generic;
using System.Linq;

using Sudoku.Cells;
using Sudoku.Puzzles;

namespace SudokuMinimizer
{
    class OneOptionPuzzleMinimizeStrategy : IPuzzleMinimizeStrategy
    {
        public OneOptionPuzzleMinimizeStrategy(int failureLimit)
        {
            FailureLimit = failureLimit;
        }

        public int FailureLimit { get; private set; }

        public Puzzle Minimize(Puzzle puzzle, int minSize)
        {
            if (!RecursiveSolver.UniqueSolution(puzzle)) // if the input doesn't have a unique solution, return null
            {
                return null;
            }

            Puzzle p = puzzle.Clone();
            bool uniqueSolution = false;
            while (!uniqueSolution)
            {
                p = puzzle.Clone();
                IList<SudokuCell> cells = p.GetAllCells().Where(x => x.Value != null).ToList();
                cells.Shuffle();
                foreach (SudokuCell c in cells)
                {
                    if (p.Count <= minSize)
                    {
                        break;
                    }
                    int val = (int)c.Value;
                    c.Value = null;
                    if (c.Options.Count() != 1)
                    {
                        c.Value = val;
                    }
                }
                uniqueSolution = RecursiveSolver.UniqueSolution(p); // If there is not a unique solution after all this, just restart
            }
            return p;
        }
    }
}
