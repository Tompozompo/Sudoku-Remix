using Sudoku.Cells;
using Sudoku.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace SudokuMinimizer
{
    static class RecursiveSolver
    {
        public static HashSet<Puzzle> Solve(Puzzle p, bool quick = true)
        {
            HashSet<Puzzle> solutions = new HashSet<Puzzle>();
            Puzzle clone = p.Clone();
            clone.Difficulty = DifficultyOption.Master;
            Solve(clone, solutions, quick);
            return solutions;
        }

        private static void Solve(Puzzle p, HashSet<Puzzle> solutions, bool quick)
        {
            Puzzle clone = p.Clone();
            SudokuCell c = GetFirstNull(clone);
            if (c == null) // no nulls remain, check if this is a solution
            {
                if (clone.IsComplete())
                {
                    solutions.Add(clone);
                }
            }
            else
            {
                IEnumerable<int> options = quick ? c.Options : p.PossibleValues;
                foreach (int o in options)
                {
                    c.Value = o;
                    if (clone.IsPossible() && (quick || clone.IsValid())) // the quick version should not choose invalid puzzles
                    {
                        Solve(clone, solutions, quick);
                    }
                    c.Value = null;
                }
            }
        }

        public static bool UniqueSolution(Puzzle p, bool quick = true)
        {
            int solutionCount = 0;
            Puzzle clone = p.Clone();
            clone.Difficulty = DifficultyOption.Master;
            return UniqueSolution(clone, quick, ref solutionCount);
        }

        private static bool UniqueSolution(Puzzle p, bool quick, ref int solutionCount)
        {
            if (solutionCount > 1) // too many solutions, immediately end
            {
                return false;
            }

            Puzzle clone = p.Clone();
            SudokuCell c = GetFirstNull(clone);
            if (c == null) // no nulls remain, check if this is a solution
            {
                if (clone.IsComplete())
                {
                    solutionCount++;
                }
            }
            else
            {
                IEnumerable<int> options = quick ? c.Options : p.PossibleValues;
                foreach (int o in options)
                {
                    c.Value = o;
                    if (clone.IsPossible() && (quick || clone.IsValid())) // the quick version should not choose invalid puzzles
                    {
                        UniqueSolution(clone, quick, ref solutionCount);
                        if (solutionCount > 1) // too many solutions, immediately end
                        {
                            return false;
                        }
                    }
                    c.Value = null;
                }
            }

            if (solutionCount == 1) //only 1 solution!
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool NoSolution(Puzzle p, int r, int c, int v, bool quick = true)
        {
            int solutionCount = 0;
            Puzzle clone = p.Clone();
            clone.Difficulty = DifficultyOption.Easy;
            return NoSolution(clone, r, c, v, quick, ref solutionCount);
        }

        private static bool NoSolution(Puzzle p, int row, int col, int value, bool quick, ref int solutionCount)
        {
            if (solutionCount > 0) // too many solutions, immediately end
            {
                return false;
            }

            Puzzle clone = p.Clone();
            SudokuCell c = GetFirstNull(clone);
            if (c == null) // no nulls remain, check if this is a solution
            {
                if (clone.IsComplete())
                {
                    solutionCount++;
                }
            }
            else
            {
                IEnumerable<int> options = quick ? c.Options : p.PossibleValues;
                foreach (int o in options)
                {
                    if(c.Row == row && c.Col == col && o == value) // skip the passed in option
                        continue;

                    c.Value = o;
                    if (clone.IsPossible() && (quick || clone.IsValid())) // the quick version should not choose invalid puzzles
                    {
                        NoSolution(clone, row, col, value, quick, ref solutionCount);
                        if (solutionCount > 0) // too many solutions, immediately end
                        {
                            return false;
                        }
                    }
                    c.Value = null;
                }
            }

            if (solutionCount == 0) // no solution found, good
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Puzzle RandomSolution(Puzzle p, bool quick = true)
        {
            Puzzle clone = p.Clone();
            SudokuCell c = GetFirstNull(clone);
            if (c == null) // no nulls remain, check if this is a solution
            {
                if (clone.IsComplete())
                {
                    return clone;
                }
            }
            else
            {
                IList<int> options = (quick ? c.Options : p.PossibleValues).ToList();
                options.Shuffle();
                foreach (int o in options)
                {
                    c.Value = o;
                    if (clone.IsPossible() && (quick || clone.IsValid())) // the quick version should not choose invalid puzzles
                    {
                        var result = RandomSolution(clone, quick);
                        if (result != null) // Found a solution!
                        {
                            return result;
                        }
                    }
                    c.Value = null;
                }
            }
            return null; // no solution found
        }

        public static SudokuCell GetFirstNull(Puzzle p)
        {
            for (int i = 0; i < p.Size; i++)
            {
                var row = p.GetRow(i);
                foreach (var r in row)
                {
                    if (r.Value == null)
                        return r;
                }
            }
            return null;
        }
    }
}
