using Sudoku.Rules;
using System;

namespace Sudoku.Puzzles
{
    [Serializable]
    public class ClassicPuzzle : Puzzle
    {

        public ClassicPuzzle(int size, int internalSize) : base(size, internalSize)
        {
            SetRules();
        }

        protected override Puzzle GetInstance()
        {
            return new ClassicPuzzle(Size, InternalSize);
        }

        private void SetRules()
        {
            // Rows
            for (int i = 0; i < Size; i++)
            {
                var row = GetRow(i);
                Rule rule = new UniqueRule(row, PossibleValues);
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
                Rule rule = new UniqueRule(col, PossibleValues);
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
                    Rule rule = new UniqueRule(box, PossibleValues);
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
