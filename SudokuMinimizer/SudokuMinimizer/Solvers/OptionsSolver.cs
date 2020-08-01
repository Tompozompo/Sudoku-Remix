using System.Collections.Generic;
using System.Linq;

using Sudoku.Cells;
using Sudoku.Puzzles;

namespace SudokuMinimizer.Solvers
{
    public class Move 
    {
        public Move(DifficultyOption diff, SudokuCell c)
        {
            Difficulty = diff;
            Cell = c;
        }

        public DifficultyOption Difficulty { get; private set; }

        public SudokuCell Cell { get; private set; }

        public override string ToString()
        {
            return Difficulty.ToString() + " : "  + Cell.ToString();
        }
    }

    public static class OptionsSolver 
    {
        public static IList<Move> Solve(Puzzle p)
        {
            IList<Move> moves = new List<Move>();
            DifficultyOption[] difficultyOptions = new DifficultyOption[4] { DifficultyOption.Easy, DifficultyOption.Medium, DifficultyOption.Hard, DifficultyOption.Expert };
            Puzzle clone = p.Clone();
            while (clone.Count < clone.Size * clone.Size) // while the puzzle isn't full
            {
                SudokuCell cell = null;
                foreach (DifficultyOption d in difficultyOptions) // check the options for each difficulty
                {
                    clone.Difficulty = d;
                    clone.ComputeOptions();
                    var foundMoves = clone.GetAllCells().Where(x => x.Options.Count() == 1).ToList();
                    if (foundMoves.Any()) // found a cell with only 1 options, set that value
                    {
                        cell = foundMoves.First();
                        cell.Value = cell.Options.First();
                        moves.Add(new Move(d, cell));
                        break;
                    }
                }
                if (cell == null)  // no valid cell was found, going to have to guess
                {
                    cell = clone.GetAllCells().First(x => x.Value == null);
                    moves.Add(new Move(DifficultyOption.Master, cell));
                    IList<int> options = cell.Options.ToList();
                    options.Shuffle();
                    foreach (int o in options)
                    {
                        cell.Value = o;
                        var result = Solve(clone);
                        if (result != null) // Found a solution!
                        {
                            return moves.Concat(result).ToList();
                        }
                        cell.Value = null;
                    }
                    return null; // no solution found 
                }
            }
            if (!clone.IsComplete())
            {
                moves = null; // after filling the board in, there is some error
            }
            return moves;
        }
    }
}
