using Sudoku.Puzzles;

namespace SudokuMinimizer
{
    interface IPuzzleMinimizeStrategy
    {
        public Puzzle Minimize(Puzzle puzzle, int minSize);
    }
}
