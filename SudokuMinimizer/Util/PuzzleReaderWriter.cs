using Sudoku.Puzzles;
using System.IO;
using System.Linq;
using System.Data.SQLite;

namespace SudokuMinimizer
{
    static class PuzzleReaderWriter
    {
        private const string ROOT = "output";

        public static string Write(Puzzle p, Puzzle sol)
        {
            // Create the folder if it doens't exist
            var typeFolder = Path.Combine(ROOT, p.GetType().Name.ToString());
            Directory.CreateDirectory(typeFolder);
            var folder = Path.Combine(typeFolder, p.Count.ToString());
            Directory.CreateDirectory(folder);

            // Find the next filename 
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.bin");
            int max = -1;
            foreach (FileInfo info in Files)
            {
                string[] nameSplit = info.Name.Split('.');
                int n = int.Parse(nameSplit[0]);
                if (n > max)
                    max = n;
            }
            max++;

            // Save the puzzle
            string filename = max.ToString() + ".txt";
            string filepath = Path.Join(folder, filename);
            using (StreamWriter writeFileStream = new StreamWriter(filepath))
            {
                Serialize(writeFileStream, p, sol);
            }

            return filepath;
        }

        public static Puzzle[] Read(string path, PuzzleFactory.PuzzleType type)
        {
            Puzzle p = PuzzleFactory.GetEmptyPuzzle(9, 3, type);
            Puzzle sol = PuzzleFactory.GetEmptyPuzzle(9, 3, type);
            using (StreamReader open = new StreamReader(path))
            {
                string line;
                while (!string.IsNullOrEmpty(line = open.ReadLine()))
                {
                    string[] s = line.Split(",");
                    int r = int.Parse(s[0]);
                    int c = int.Parse(s[1]);
                    int v = int.Parse(s[2]);
                    p[r, c].Value = v;
                }

                while(!string.IsNullOrEmpty(line = open.ReadLine()))
                {
                    string[] s = line.Split(",");
                    int r = int.Parse(s[0]);
                    int c = int.Parse(s[1]);
                    int v = int.Parse(s[2]);
                    sol[r, c].Value = v;
                }
            }
            return new Puzzle[] { p, sol };
        }

        private static void Serialize(StreamWriter stream, Puzzle p, Puzzle sol)
        {
            foreach(var cell in p.GetAllCells().Where(x => x.Value != null))
            {
                stream.WriteLine(string.Format("{0},{1},{2}", cell.Row, cell.Col, cell.Value));
            }
            stream.WriteLine(string.Format(""));
            foreach (var cell in sol.GetAllCells().Where(x => x.Value != null))
            {
                stream.WriteLine(string.Format("{0},{1},{2}", cell.Row, cell.Col, cell.Value));
            }
            stream.WriteLine(string.Format(""));
            foreach (var rule in p.PuzzleRules)
            {
                stream.WriteLine(rule.Serialize());
            }
        }
    }
}
