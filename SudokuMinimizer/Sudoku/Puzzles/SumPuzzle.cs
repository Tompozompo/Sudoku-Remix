using Sudoku.Rules;

namespace Sudoku.Puzzles
{
    public class SumPuzzle : Puzzle
    {
        public SumPuzzle(int size, int internalSize, int sum) : base(size, internalSize)
        {
            Sum = sum;
            SetRules();
        }

        public int Sum { get; private set; }

        protected override Puzzle GetInstance()
        {
            return new SumPuzzle(Size, InternalSize, Sum);
        }

        protected void SetRules()
        {
            // Rows
            for (int i = 0; i < Size; i++)
            {
                var row = GetRow(i);
                Rule rule = new SumRule(row, PossibleValues, Sum);
                PuzzleRules.Add(rule);
                foreach (var r in row)
                {
                    r.Rules.Add(rule);
                }
            }

            // Columns
            for (int i = 0; i < Size; i++)
            {
                var col = GetCol(i);
                Rule rule = new SumRule(col, PossibleValues, Sum);
                PuzzleRules.Add(rule);
                foreach (var c in col)
                {
                    c.Rules.Add(rule);
                }
            }

            // 3x3 cells
            for (int i = 0; i < InternalSize; i++)
            {
                for (int j = 0; j < InternalSize; j++)
                {
                    var box = GetBox(i, j);
                    Rule rule = new SumRule(box, PossibleValues, Sum);
                    PuzzleRules.Add(rule);
                    foreach (var b in box)
                    {
                        b.Rules.Add(rule);
                    }
                }
            }
        }
    }
}
