using Sudoku.Cells;
using Sudoku.Puzzles;
using System.Linq;

namespace SudokuMinimizer
{
    class UniquSolutionPuzzleMinimizeStrategy : IPuzzleMinimizeStrategy
    {

        public UniquSolutionPuzzleMinimizeStrategy(int failureLimit, int puzzleMin)
        {
            FailureLimit = failureLimit;
            PuzzleMin = puzzleMin;
        }
        
        public int FailureLimit { get; private set; }

        public int PuzzleMin { get; private set; }

        public Puzzle Minimize(Puzzle puzzle, int minSize)
        {
            if (!RecursiveSolver.UniqueSolution(puzzle)) // if the input doesn't have a unique solution, return null
            {
                return null;
            }

            Puzzle p = puzzle.Clone();
            int failCount = 0;
            while (failCount < FailureLimit) // Randomly remove cells while maintaining 1 unique solution
            {
                SudokuCell c = p.GetAllCells().Where(x => x.Value != null).ToList().GetRandomElement();
                int val = (int)c.Value;
                c.Value = null;
                bool unique = RecursiveSolver.UniqueSolution(p);
                if (!unique)
                {
                    c.Value = val;
                    failCount++;
                }
                else
                {
                    failCount = 0;
                }
            }
            for (int i = 0; i < p.Size; i++) // After randomly removing fails, check every cell to be sure we didnt miss any
            {
                for (int j = 0; j < p.Size; j++)
                {
                    if (p.Count <= PuzzleMin) // If we reached the min, stop early
                    {
                        break;
                    }
                    SudokuCell c = p[i, j];
                    if (c.Value == null)
                    {
                        continue;
                    }
                    int val = (int)c.Value;
                    c.Value = null;
                    bool unique = RecursiveSolver.UniqueSolution(p);
                    if (!unique)
                    {
                        c.Value = val;
                    }
                }
            }
            return p;
        }
    }
}
