using System.Linq;

using Sudoku.Puzzles;
using Sudoku.Rules;

namespace SudokuMinimizer
{
    class PuzzleFactory
    {
        public enum PuzzleType
        {
            Classic,
            Sum,
            DiagonalSum,
            Knights,
            Kings,
            KingKnights,
            MagicSquare
        }

        public static Puzzle GetEmptyPuzzle(int size, int internalSize, PuzzleType type)
        {
            return type switch
            {
                PuzzleType.Classic => new ClassicPuzzle(size, internalSize),
                PuzzleType.Sum => new SumPuzzle(size, internalSize, 45),
                PuzzleType.DiagonalSum => new DiagonalSumPuzzle(size, internalSize),
                PuzzleType.Knights => new KnightsPuzzle(size, internalSize),
                PuzzleType.Kings => new KingsPuzzle(size, internalSize),
                PuzzleType.KingKnights => new KingKnightsPuzzle(size, internalSize),
                PuzzleType.MagicSquare => new MagicSquarePuzzle(size, internalSize),
                _ => new ClassicPuzzle(size, internalSize),
            };
        }

        public static Puzzle GetRandomSolution(int size, int internalSize, PuzzleType type)
        {
            Puzzle p = GetEmptyPuzzle(size, internalSize, type);
            p.Difficulty = DifficultyOption.Easy;
            p.ComputeOptions();
            var msrules = p.PuzzleRules.Where(x => x is MagicSquareRule).ToList();
            if (msrules.Any()) // Solve any magic squares first
            {
                var rule = msrules.First();
                foreach (var c in rule.Cells) // for each element of the square
                {
                    var opt = c.Options.ToList();
                    opt.Shuffle();
                    foreach (var v in opt) // try each option, do not take any values that make the puzzle impossible
                    {
                        c.Value = v;
                        if (!p.IsPossible())
                            c.Value = null;
                        else
                            break;
                    }
                }
            }
            p = RecursiveSolver.RandomSolution(p);
            if (p == null || !p.IsComplete())
            {
                return null;
            }
            return p;
        }
    }
}
