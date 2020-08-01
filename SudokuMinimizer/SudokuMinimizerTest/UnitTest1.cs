using NUnit.Framework;
using SudokuMinimizer;

namespace SudokuMinimizerTest
{
    public class Tests
    {
        public Puzzle puzzle;
        public int size = 3;
        public int internalSize = 3;
        IPuzzleMinimizeStrategy preMinStrat = new RandomOptionsPuzzleMinimizeStrategy(1000);
        IPuzzleMinimizeStrategy minStrat = new UniquSolutionPuzzleMinimizeStrategy(5);

        [SetUp]
        public void Setup()
        {
            puzzle = Puzzle.GetRandom(size, internalSize);
            Puzzle preMinPuzzle = preMinStrat.Minimize(puzzle);
            IPuzzleMinimizeStrategy preMinStrat = new RandomOptionsPuzzleMinimizeStrategy(1000);
            IPuzzleMinimizeStrategy minStrat = new UniquSolutionPuzzleMinimizeStrategy(5);
        }

        [Test]
        public void Test1()
        {
            HashSet<Puzzle> solutions = RecursiveSolver.Solve(minPuzzle);
            bool unique = RecursiveSolver.UniqueSolution(minPuzzle);
        }
    }
}