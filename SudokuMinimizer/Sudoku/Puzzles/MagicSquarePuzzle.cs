using Sudoku.Rules;
using System;

namespace Sudoku.Puzzles
{
    [Serializable]
    public class MagicSquarePuzzle : ClassicPuzzle
    {
        public MagicSquarePuzzle(int size, int internalSize) : base(size, internalSize)
        {
            SetRules();
        }

        protected override Puzzle GetInstance()
        {
            return new MagicSquarePuzzle(Size, InternalSize);
        }

        protected void SetRules()
        {
            var box = GetBox(1, 1);
            Rule rule = new MagicSquareRule(box, PossibleValues, InternalSize);
            PuzzleRules.Add(rule);
            foreach (var r in box)
            {
                r.Rules.Add(rule);
            }
        }
    }
}

