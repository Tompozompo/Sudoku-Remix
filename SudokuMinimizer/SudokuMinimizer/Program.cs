using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data.SQLite;

using Sudoku.Puzzles;
using Sudoku.Rules;
using SudokuMinimizer.Solvers;
using static SudokuMinimizer.PuzzleFactory;
using SudokuMinimizer.Database;

namespace SudokuMinimizer
{
    class Program
    {
        // PUZZLE SIZE SETTINGS
        private static readonly int SIZE = 9;
        private static readonly int INTERNALSIZE = 3;

        // SUM RULE SETTINGS
        private static readonly int SUM = 30;
        private static readonly int MAXCELLCOUNT = 9;

        // STOPWATCH
        private static readonly Stopwatch stopWatch = new Stopwatch();

        // PUZZLE TYPE SETTING
        private static readonly PuzzleType TYPE = PuzzleType.Classic;
        private static readonly int MINSIZE = 10;
        private static readonly DifficultyOption DIFFICULTY = DifficultyOption.Master;  
        private static readonly int LIMIT = 1; 

        /* Difficulty ideas:
        DIFFICULTY = 0 + some limit - Easy
        DIFFICULTY = 0 - Med 
        DIFFICULTY = 3 - Hard 
        DIFFICULTY = 4 & UniqueSolutionPuzzleMinimizeStrategy - Expert
        */

        private static void Init()
        {
            stopWatch.Start();
            SumRule.ComputeAllWays(SUM, Enumerable.Range(1, SIZE).ToList(), MAXCELLCOUNT);
            IncreasingRule.ComputeAllWays(MAXCELLCOUNT, Enumerable.Range(1, SIZE).ToList());
            //SQLiteDatabase.DropTable();
            SQLiteDatabase.CreateTable();
        }

        static void Main(string[] args)
        {
            Init();

            for (int i = 0; i < LIMIT; i++)
            {
                // Get a random solution
                Puzzle solution = GetRandomSolution(SIZE, INTERNALSIZE, TYPE);
                if (solution == null)
                {
                    Console.WriteLine("No possible solution :(");
                    return;
                }
                if (!solution.IsValid())
                {
                    Console.WriteLine("ERROR: INVALID SOLUTION");
                    return;
                }
                Console.WriteLine("Solution:");
                IDisplayStrategy.Display(solution);
                Console.WriteLine();
                PrintTime();

                // Remove clues from the solution, maintaining a unique solution
                solution.Difficulty = DIFFICULTY;
                List<IPuzzleMinimizeStrategy> minStrats = new List<IPuzzleMinimizeStrategy>();
                if (DIFFICULTY == DifficultyOption.Master)
                {
                    minStrats.Add(new UniqueSolutionPuzzleMinimizeStrategy());
                }
                else
                {
                    minStrats.Add(new OneOptionPuzzleMinimizeStrategy(1000));
                }
                Puzzle minPuzzle = solution;
                foreach (var minStrat in minStrats)
                {
                    minPuzzle = minStrat.Minimize(minPuzzle, MINSIZE);
                }
                Console.WriteLine("Minimized puzzle: ");
                IDisplayStrategy.Display(minPuzzle);
                Console.WriteLine("Min puzzle size: " + minPuzzle.Count);
                Console.WriteLine();
                PrintTime();
                //AssertUnique(minPuzzle);

                // Save the puzzle + solution
                IList<Move> moves = OptionsSolver.Solve(minPuzzle);
                var orderedMoves = moves.OrderByDescending(x => x.Difficulty).ToList();
                SQLiteDatabase.AddPuzzle(minPuzzle, solution, moves);

                /*
                var readPuzzle = SQLiteDatabase.GetPuzzle();
                if (!minPuzzle.Equals(readPuzzle[0]))
                {
                    throw new InvalidOperationException();
                }
                if (!solution.Equals(readPuzzle[1]))
                {
                    throw new InvalidOperationException();
                }
                */
            }

            var puzzles = SQLiteDatabase.GetAllPuzzles();
            Console.WriteLine("Done. Puzzle count " + puzzles.Count);
            stopWatch.Stop();
            PrintTime();
        }

        private static void PrintTime()
        {
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        private static void AssertUnique(Puzzle p)
        {
            HashSet<Puzzle> solutions = RecursiveSolver.Solve(p, false);
            if (solutions.Count != 1)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
