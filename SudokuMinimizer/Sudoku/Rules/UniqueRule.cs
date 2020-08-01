using Sudoku.Cells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Rules
{
    [Serializable]
    class UniqueRule : Rule
    {
        public UniqueRule(IEnumerable<SudokuCell> cells, IEnumerable<int> possibleValues) : base(cells, possibleValues)
        {
        }

        public override bool IsSatisfied()
        {
            HashSet<int> foundValues = new HashSet<int>();
            foreach (SudokuCell cell in Cells)
            {
                if (cell.Value == null) // skip nulls
                {
                    continue;
                }
                else if (!PossibleValues.Contains((int)cell.Value)) // invalid value
                {
                    return false;
                }
                else if (foundValues.Contains((int)cell.Value)) // repeat value
                {
                    return false;
                }
                foundValues.Add((int)cell.Value);
            }
            return true;
        }

        public override IList<int> GetOptions(SudokuCell cell)
        {
            if (!Cells.Contains(cell))
            {
                throw new ArgumentException();
            }
            else if (cell.Value == null)
            {
                // Can't be any values that exist in another cell
                return PossibleValues.Except(Cells.Select(x => x.Value ?? 0)).ToList();
            }
            else
            {
                return new List<int>();
            }
        }

        private class IntArrayEqualityComparer : IEqualityComparer<int[]>
        {
            public bool Equals(int[] x, int[] y)
            {
                return x.Length == y.Length && !x.Except(y).Any();
            }

            public int GetHashCode(int[] obj)
            {
                int result = 17;
                for (int i = 0; i < obj.Length; i++)
                {
                    unchecked
                    {
                        result = result * 23 + obj[i];
                    }
                }
                return result;
            }
        }
    }
}
