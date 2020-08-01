namespace SudokuMinimizer.Database
{
    public struct DBPuzzle
    {
        public DBPuzzle(int id, int count, int easy, int medium, int hard, int expert, int master, string puzzle, string solution)
        {
            Id = id;
            ClueCount = count;
            EasyMoves = easy;
            MediumMoves = medium;
            HardMoves = hard;
            ExpertMoves = expert;
            MasterMoves = master;
            Puzzle = puzzle;
            Solution = solution; 
        }

        public int Id { get; set; }

        public int ClueCount { get; set; }

        public int EasyMoves { get; set; }

        public int MediumMoves { get; set; }

        public int HardMoves { get; set; }

        public int ExpertMoves { get; set; }

        public int MasterMoves { get; set; }

        public string Puzzle { get; set; }

        public string Solution { get; set; }

        public override string ToString()
        {
            return $@"{Id},{ClueCount},{EasyMoves},{MediumMoves},{HardMoves},{ExpertMoves},{MasterMoves},{Puzzle},{Solution}";
        }
    }
}
