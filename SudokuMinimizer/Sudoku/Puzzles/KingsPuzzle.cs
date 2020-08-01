using Sudoku.Cells;
using Sudoku.Rules;
using System;
using System.Collections.Generic;

namespace Sudoku.Puzzles
{
    [Serializable]
    public class KingsPuzzle : ClassicPuzzle
    {
        private static readonly IList<Tuple<int, int>> moves = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(- 1, - 1),
            new Tuple<int, int>(- 1,   0),
            new Tuple<int, int>(- 1, + 1),
            new Tuple<int, int>(  0, - 1),
            new Tuple<int, int>(  0, + 1),
            new Tuple<int, int>(+ 1, - 1),
            new Tuple<int, int>(+ 1,   0),
            new Tuple<int, int>(+ 1, + 1)
        };

        public KingsPuzzle(int size, int internalSize) : base(size, internalSize)
        {
            SetRules();
        }

        protected override Puzzle GetInstance()
        {
            return new KingsPuzzle(Size, InternalSize);
        }

        private void SetRules()
        {
            foreach (SudokuCell c in GetAllCells())
            {
                var cells = new List<SudokuCell>();
                foreach (var move in moves)
                {
                    int row = c.Row + move.Item1;
                    int col = c.Col + move.Item2;
                    if (row >= 0 && row < Size && col >= 0 && col < Size)
                    {
                        cells.Add(this[row, col]);
                    }
                }

                Rule rule = new UniqueMainCellRule(cells, PossibleValues, c);
                PuzzleRules.Add(rule);
                c.Rules.Add(rule);
                foreach (var k in cells)
                {
                    k.Rules.Add(rule);
                }
            }
        }
    }
}
