using System;
using System.Collections.Generic;

using Sudoku.Cells;

namespace Sudoku.Rules
{
    [Serializable]
    public abstract class Rule
    {
        public Rule(IEnumerable<SudokuCell> cells, IEnumerable<int> possibleValues)
        {
            Cells = cells;
            PossibleValues = possibleValues;
        }
        
        public IEnumerable<SudokuCell> Cells { get; private set; }
        
        public IEnumerable<int> PossibleValues { get; private set; }

        public abstract bool IsSatisfied();
        
        public abstract IList<int> GetOptions(SudokuCell cell);

        public virtual string Serialize()
        {
            string res = string.Format("{0}&", GetType().Name);
            foreach (var cell in Cells)
            {
                res += string.Format("{0},{1}|", cell.Row, cell.Col);
            }
            return res.Remove(res.Length-1);
        }
    }
}
