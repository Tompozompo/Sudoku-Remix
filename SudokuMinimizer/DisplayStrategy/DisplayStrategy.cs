using Sudoku.Puzzles;

namespace SudokuMinimizer
{
    public interface IDisplayStrategy
    {
        private static IDisplayStrategy impl = new OptionsCmdDisplayStrategy(); 

        public void DisplayImpl(Puzzle p);

        public static void Display(Puzzle p)
        {
           impl.DisplayImpl(p);
        }
    }
}
