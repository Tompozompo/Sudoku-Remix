using System;
using System.Collections.Generic;
using System.Linq;

using Sudoku.Puzzles;
using Sudoku.Rules;

namespace Sudoku.Cells
{
    public class SudokuCell : IEquatable<SudokuCell>
    {
        internal int? _value;
        private Puzzle _parent;

        public SudokuCell(int row, int col, Puzzle parent) 
        {
            _parent = parent;
            _value = null;
            Row = row;
            Col = col;
            Rules = new List<Rule>();
        }

        public int Row { get; internal set; }

        public int Col { get; internal set; }

        public int? Value { 
            get 
            { 
                return _value; 
            }
            set 
            {
                _value = value;
                _parent.ComputeOptions();
            } 
        }


        public IList<int> Options { get; internal set; }

        public IList<Rule> Rules { get; internal set; }

        public void ComputeOptions()
        {
            var options = _parent.PossibleValues;
            foreach (Rule r in Rules)
            {
                options = r.GetOptions(this).Intersect(options);
            }
            Options = options.ToList();
        }

        public bool Equals(SudokuCell other)
        {
            return other != null && other.Row == Row && other.Col == Col && other.Value == Value;
        }

        internal void CopyFrom(SudokuCell cell)
        {
            _value = cell.Value;
            Options = new List<int>(cell.Options);
        }

        public override int GetHashCode()
        {
            return Row + Col + (Value ?? 0);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
