using System;
using System.Collections.Generic;
using System.Linq;

using Sudoku.Cells;
using Sudoku.Util;

namespace Sudoku.Rules
{
    [Serializable]
    public class SumRule : Rule
    {
        private static HashSet<IList<int>>[] allWays;

        public SumRule(IEnumerable<SudokuCell> cells, IEnumerable<int> possibleValues, int sum) : base(cells, possibleValues)
        {
            Sum = sum;
        }

        public int Sum { get; private set; }

        public int CurrentTotal
        {
            get
            {
                return Cells.Sum(x => x.Value ?? 0);
            }
        }

        public override bool IsSatisfied()
        {
            return CurrentTotal <= Sum;
        }

        public override IList<int> GetOptions(SudokuCell cell)
        {
            if (!Cells.Contains(cell))
            {
                throw new ArgumentException();
            }
            else if (cell.Value == null)
            {
                int difference = Sum - CurrentTotal;
                int remaining = Cells.Where(x => x.Value == null).Count();
                var matchingWays = allWays[difference].Where(x => x.Count == remaining).ToList();
                var values = matchingWays.SelectMany(x => x).ToList();
                IList<int> res = PossibleValues.Intersect(values).ToList();
                return res;
            }
            else
            {
                return new List<int>();
            }
        }

        public static void ComputeAllWays(int maxSum, IList<int> possibleValues, int maxCellCount)
        {
            allWays = FindAllWays(maxSum, possibleValues, maxCellCount);
        }

        private static HashSet<IList<int>>[] FindAllWays(int sum, IList<int> possibleValues, int maxCellCount)
        {
            HashSet<IList<int>>[] ways = new HashSet<IList<int>>[sum + 1];

            ways[0] = new HashSet<IList<int>>();
            ways[0].Add(new List<int>());

            for (int i = 1; i <= sum; i++)
            {
                HashSet<IList<int>> set = new HashSet<IList<int>>(new SameElementsEqualityComparer<int>());
                foreach (int value in possibleValues)
                {
                    if (i >= value)
                    {
                        HashSet<IList<int>> subWays = ways[i - value];
                        foreach (IList<int> way in subWays)
                        {
                            if (way.Count < maxCellCount)
                            {
                                IList<int> clone = new List<int>(way);
                                clone.Add(value);
                                set.Add(clone);
                            }
                        }
                    }
                }
                ways[i] = set;
            }
            return ways;
        }

        public override string Serialize()
        {
            return base.Serialize() + "&" + Sum;
        }

        private class SumList : IEquatable<SumList>
        {
            public SumList()
            {
                Numbers = new List<int>();
            }

            public SumList(IList<int> numbers)
            {
                Numbers = numbers;
            }

            public IList<int> Numbers { get; set; }
            
            public int Count { get { return Numbers.Count; } }

            public void Add(int i)
            {
                Numbers.Add(i);
            }

            public bool Equals(SumList other)
            {
                return (Count == other.Count) && !Numbers.Except(other.Numbers).Any();
            }

            public override int GetHashCode()
            {
                return Numbers.Sum();
            }

            public SumList Clone()
            {
                return new SumList(Numbers.ToList());
            }

        }
    }
}

