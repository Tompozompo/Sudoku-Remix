using Sudoku.Cells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Rules
{
    [Serializable]
    class UniqueMainCellRule : Rule
    {
        public UniqueMainCellRule(IEnumerable<SudokuCell> cells, IEnumerable<int> possibleValues, SudokuCell main) : base(cells, possibleValues)
        {
            MainCell = main;
        }

        public SudokuCell MainCell { get; private set; }

        public override bool IsSatisfied()
        {
            if (MainCell.Value == null)
            {
                return true;
            }
            foreach (SudokuCell cell in Cells)
            {
                if (cell.Value == null) // skip nulls
                {
                    continue;
                }
                else if (cell.Value == MainCell.Value) // repeat value
                {
                    return false;
                }
            }
            return true;
        }

        public override IList<int> GetOptions(SudokuCell cell)
        {
            if (MainCell != cell && !Cells.Contains(cell))
            {
                throw new ArgumentException();
            }
            else if (cell.Value == null)
            {
                IList<int> res;
                if (cell == MainCell)
                {
                    res = PossibleValues.Except(Cells.Select(x => x.Value ?? 0)).ToList();
                }
                else // in Cells
                {
                    res = PossibleValues.Except(new List<int> (MainCell.Value ?? 0)).ToList();
                }
                return res;
            }
            else
            {
                return new List<int>();
            }
        }

        public override string Serialize()
        {
            return base.Serialize() + string.Format("&{0},{1}", MainCell.Row, MainCell.Col);
        }
    }
}
