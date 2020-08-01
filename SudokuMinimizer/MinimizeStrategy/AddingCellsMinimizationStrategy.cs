using Sudoku.Cells;
using Sudoku.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuMinimizer
{
    class AddingCellsMinimizationStrategy : IPuzzleMinimizeStrategy
    {
        public AddingCellsMinimizationStrategy(int startingMin)
        {
            StartingMinimum = startingMin;
        }

        public int StartingMinimum { get; private set; }

        public Puzzle Minimize(Puzzle puzzle, int minSize)
        {
            if (!RecursiveSolver.UniqueSolution(puzzle)) // if the input doesn't have a unique solution, return null
            {
                return null;
            }

            Puzzle p = puzzle.Clone();
            while (StartingMinimum < p.Count) // Randomly remove cells until we are at the bottom
            {
                SudokuCell c = p.GetAllCells().Where(x => x.Value != null).ToList().GetRandomElement();
                c.Value = null;
            }

            HashSet<Puzzle> solutions = RecursiveSolver.Solve(p);
            while (solutions.Count > 1) // While there is not a unique solution, add more cells
            {
                Dictionary<Tuple<int, int>, Dictionary<int, int>> counter = new Dictionary<Tuple<int, int>, Dictionary<int, int>>(); // counter[cell(i,j)][cell.value] = count
                for (int i = 0; i < p.Size; i++) // Init the counter
                {
                    for (int j = 0; j < p.Size; j++)
                    {
                        var key = new Tuple<int, int>(i, j);
                        var value = new Dictionary<int, int>();
                        foreach (var s in p.PossibleValues)
                        {
                            value[s] = 0;
                        }
                        counter[key] = value;
                    }
                }

                foreach (var solution in solutions) // Count them up
                {
                    for (int i = 0; i < p.Size; i++)
                    {
                        for (int j = 0; j < p.Size; j++)
                        {
                            Tuple<int, int> t = new Tuple<int, int>(i, j);
                            int s = (int)solution[i, j].Value;
                            counter[t][s]++;
                        }
                    }
                }

                SudokuCell minCell = null;
                int minValue = 0;
                int minCount = solutions.Count;
                foreach (var cellCounter in counter)  // Pick the cell and value that has the least count
                {
                    Tuple<int, int> cellCoord = cellCounter.Key;
                    foreach (var valueCount in cellCounter.Value)
                    {
                        int cellValue = valueCount.Key;
                        int count = valueCount.Value;
                        if (count != 0 && count < minCount)
                        {
                            minCount = count;
                            minValue = cellValue;
                            minCell = p[cellCoord.Item1, cellCoord.Item2];
                        }
                        if (minCount == 1)
                        {
                            break; // can just stop now, can't get better
                        }
                    }
                }
                minCell.Value = minValue;
                solutions = RecursiveSolver.Solve(p);
            }
            return p;
        }
    }
}
