using Sudoku.Puzzles;
using SudokuMinimizer.Solvers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using static SudokuMinimizer.Database.UtilFunctions;

namespace SudokuMinimizer.Database
{
    public static class SQLiteDatabase
    {
        private static readonly string connectionString = @"URI=file:C:\src\sudokuUI\sudoku\assets\puzzles.db";

        public static void DropTable()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();

            using var cmd = new SQLiteCommand(con)
            {
                CommandText = @"DROP TABLE IF EXISTS puzzles"
            };
            cmd.ExecuteNonQuery();

            Console.WriteLine("Table puzzles dropped");
        }

        public static void CreateTable()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();

            using var cmd = new SQLiteCommand(con)
            {
                CommandText = @"CREATE TABLE IF NOT EXISTS puzzles(id INTEGER PRIMARY KEY, count INTEGER, easy INTEGER, medium INTEGER, hard INTEGER, expert INTEGER, master INTEGER, puzzle TEXT, solution TEXT)"
            };
            cmd.ExecuteNonQuery();

            Console.WriteLine("Table puzzles created");
        }

        public static List<DBPuzzle> GetAllPuzzles()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();

            using var cmd = new SQLiteCommand(con)
            {
                CommandText = "SELECT * FROM puzzles"
            };
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            List<DBPuzzle> puzzles = new List<DBPuzzle>();
            while (rdr.Read())
            {
                int id = DbInt(rdr["id"]);
                int count = DbInt(rdr["count"]);
                int easy = DbInt(rdr["easy"]);
                int medium = DbInt(rdr["medium"]);
                int hard = DbInt(rdr["hard"]);
                int expert = DbInt(rdr["expert"]);
                int master = DbInt(rdr["master"]);
                string puzzle = DbString(rdr["puzzle"]);
                string solution = DbString(rdr["solution"]);

                puzzles.Add(new DBPuzzle(id, count, easy, medium, hard, expert, master, puzzle, solution));
            }
            return puzzles;
        }

        public static void AddPuzzle(Puzzle puzzle, Puzzle solution, IList<Move> moves)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();

            int count = puzzle.Count;
            int easy = moves.Where(x => x.Difficulty == DifficultyOption.Easy).Count();
            int medium = moves.Where(x => x.Difficulty == DifficultyOption.Medium).Count();
            int hard = moves.Where(x => x.Difficulty == DifficultyOption.Hard).Count();
            int expert = moves.Where(x => x.Difficulty == DifficultyOption.Expert).Count();
            int master = moves.Where(x => x.Difficulty == DifficultyOption.Master).Count();
            string puzzleSerial = Serialize(puzzle);
            string solutionSerial = Serialize(solution);
            string cmdbase = "INSERT INTO puzzles(count, easy, medium, hard, expert, master, puzzle, solution) VALUES({0},{1},{2},{3},{4},{5},\"{6}\",\"{7}\")";

            using var cmd = new SQLiteCommand(con)
            {
                CommandText = string.Format(cmdbase, count, easy, medium, hard, expert, master, puzzleSerial, solutionSerial)
            };
            cmd.ExecuteNonQuery();
        }
        
        public static Puzzle[] GetPuzzle()
        {
            var puzzles = GetAllPuzzles();
            puzzles.Shuffle();
            var p = puzzles.FirstOrDefault();
            return new Puzzle[2] { Deserialize(p.Puzzle), Deserialize(p.Solution) };
        }

        private static string Serialize(Puzzle p)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var cell in p.GetAllCells())
            {
                builder.Append(string.Format("{0}", cell.Value?.ToString() ?? "-"));
            }
            return builder.ToString();
        }

        private static Puzzle Deserialize(string serial)
        {
            Puzzle p = new ClassicPuzzle(9, 3);
            int i = 0;
            foreach(char c in serial)
            {
                int row = i / 9;
                int col = i % 9;
                i++;
                if (c == '-')
                {
                    continue;
                }
                else
                {
                    p[row, col].Value = int.Parse(c.ToString());
                }
            }
            return p;
        }
    }
}
