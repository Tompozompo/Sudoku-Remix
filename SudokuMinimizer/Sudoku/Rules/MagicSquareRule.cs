using Sudoku.Cells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Rules
{
    [Serializable]
    public class MagicSquareRule : Rule
    {
        public MagicSquareRule(IEnumerable<SudokuCell> cells, IEnumerable<int> possibleValues, int size) : base(cells, possibleValues)
        {
            Size = size;
            if (Size != 3) // don't feel like making GetOptions intelligent to handle non 3x3 squares
            {
                throw new NotImplementedException();
            }
            Square = new SudokuCell[size, size];
            Sum = possibleValues.Sum() / size;
            int count = 0;
            foreach (var e in cells)
            {
                Square[count / size, count % size] = e;
                count++;
            }
        }

        public int Sum { get; private set; }
        
        public SudokuCell[,] Square { get; private set; }

        public int Size { get; private set; }

        public override IList<int> GetOptions(SudokuCell cell)
        {
            if (!Cells.Contains(cell))
            {
                throw new ArgumentException();
            }
            else if (cell.Value == null)
            {
                IList<int> res;
                // Hardcoded for a 3x3 square
                // To make this better, should compute the options based on how many sums the cell must satisfy.
                // E.G., the center is used in 4 different sums. Looking at all possible ways to sum to 15, only 5 is used in 4 different sums.
                IList<SudokuCell> corners = new List<SudokuCell> { Square[0, 0], Square[2, 2], Square[0, 2], Square[2, 0] };
                IList<SudokuCell> edges = new List<SudokuCell> { Square[0, 1], Square[1, 0], Square[1, 2], Square[2, 1] };
                var r0 = GetSquareRow(0);
                var r1 = GetSquareRow(1);
                var r2 = GetSquareRow(2);
                var c0 = GetSquareCol(0);
                var c1 = GetSquareCol(1);
                var c2 = GetSquareCol(2);
                var dd = GetSquareDiagD();
                var du = GetSquareDiagU();
                if (cell == Square[1, 1]) // is center
                {
                    res = new List<int> { 5 };
                }
                else if (r0.Contains(cell) && r0.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - r0.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (r1.Contains(cell) && r1.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - r1.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (r2.Contains(cell) && r2.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - r2.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (c0.Contains(cell) && c0.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - c0.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (c1.Contains(cell) && c1.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - c1.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (c2.Contains(cell) && c2.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - c2.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (dd.Contains(cell) && dd.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - dd.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (du.Contains(cell) && du.Where(x => x.Value != null || x == Square[1, 1]).Count() == 2)
                {
                    res = new List<int> { Sum - du.Sum(x => x == Square[1, 1] ? 5 : x.Value ?? 0) };
                }
                else if (corners.Contains(cell))
                {
                    res = new List<int> { 2, 4, 6, 8 };
                }
                else
                {
                    res = new List<int> { 1, 3, 7, 9 };
                }
                return res;
            }
            else
            {
                return new List<int>();
            }
        }

        public override bool IsSatisfied()
        {
            // Rows
            for (int i = 0; i < Size; i++)
            {
                var row = GetSquareRow(i);
                if (row.Sum(x => x.Value ?? 0) > Sum)
                {
                    return false;
                }
            }

            // Columns
            for (int i = 0; i < Size; i++)
            {
                var col = GetSquareCol(i);
                if (col.Sum(x => x.Value ?? 0) > Sum)
                {
                    return false;
                }
            }

            // Diagonals
            var diagD = GetSquareDiagD();
            if (diagD.Sum(x => x.Value ?? 0) > Sum)
            {
                return false;
            }

            var diagU = GetSquareDiagU();
            if (diagU.Sum(x => x.Value ?? 0) > Sum)
            {
                return false;
            }

            return true;
        }

        private IList<SudokuCell> GetSquareCol(int columnNumber)
        {
            return Enumerable.Range(0, Size)
                    .Select(x => Square[x, columnNumber]).ToList();
        }

        private IList<SudokuCell> GetSquareRow(int rowNumber)
        {
            return Enumerable.Range(0, Size)
                    .Select(x => Square[rowNumber, x]).ToList();
        }

        public IList<SudokuCell> GetSquareDiagD()
        {
            return Enumerable.Range(0, Size)
                    .Select(x => Square[x, x]).ToList();
        }

        public IList<SudokuCell> GetSquareDiagU()
        {
            return Enumerable.Range(0, Size)
                    .Select(x => Square[x, Size - x - 1]).ToList();
        }
    }
}
