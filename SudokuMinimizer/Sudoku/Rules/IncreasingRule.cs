using System;
using System.Collections.Generic;
using System.Linq;

using Sudoku.Cells;

namespace Sudoku.Rules
{
    [Serializable]
    public class IncreasingRule : Rule
    {
        private static HashSet<IList<int?>>[] allWays;

        public IncreasingRule(IEnumerable<SudokuCell> cells, IEnumerable<int> possibleValues) : base(cells, possibleValues)
        {
        }

        public override IList<int> GetOptions(SudokuCell cell)
        {
            if (!Cells.Contains(cell))
            {
                throw new ArgumentException();
            }
            else if (cell.Value == null)
            {
                var ways = allWays[Cells.Count()].AsEnumerable();
                for(int i = 0; i < Cells.Count(); i++)
                {
                    var c = Cells.ToList()[i];
                    if (c.Value != null)
                    {
                        ways = ways.Where(x => x[i] == c.Value);
                    }
                }
                IList<int> res = PossibleValues.ToList();
                return res;
            }
            else
            {
                return new List<int>();
            }
        }

        public override bool IsSatisfied()
        {
            var sorted = Cells.OrderBy(x => x.Value);
            return sorted.SequenceEqual(Cells);
        }

        public static void ComputeAllWays(int maxLength, IList<int> possibleValues)
        {
            allWays = FindAllWays(maxLength, possibleValues);
        }

        private static HashSet<IList<int?>>[] FindAllWays(int maxLength, IList<int> possibleVals)
        {
            HashSet<IList<int?>>[] ways = new HashSet<IList<int?>>[maxLength + 1];
            ways[0] = new HashSet<IList<int?>>();
            ways[0].Add(new List<int?>());
            IList<int?> possibleValues = new List<int?>(possibleVals.Select(x => (int?)x));
            possibleValues.Add(null);

            for (int length = 1; length <= maxLength; length++)
            {
                HashSet<IList<int?>> set = new HashSet<IList<int?>>();
                foreach (int? value in possibleValues)
                {
                    HashSet<IList<int?>> subWays = ways[length - 1];
                    foreach (IList<int?> way in subWays)
                    {
                        int? max = way.Count == 0 ? possibleValues.Min() - 1 : way.Max() ?? possibleValues.Min() - 1;
                        if (value == null || max < value) // Add this value after the 
                        {
                            IList<int?> clone1 = new List<int?>(way);
                            clone1.Add(value);
                            set.Add(clone1);
                        }
                    }
                }
                ways[length] = set;
            }
            return ways;
        }

        private class PossibleOrder : IEquatable<PossibleOrder>
        {
            public PossibleOrder()
            {
                Numbers = new List<int>();
            }

            public PossibleOrder(IList<int> numbers)
            {
                Numbers = numbers;
            }

            public IList<int> Numbers { get; set; }

            public int Count { get { return Numbers.Count; } }

            public void Add(int i)
            {
                Numbers.Add(i);
            }

            public bool Equals(PossibleOrder other)
            {
                return (Count == other.Count) && !Numbers.Except(other.Numbers).Any();
            }

            public override int GetHashCode()
            {
                return Numbers.Sum();
            }

            public PossibleOrder Clone()
            {
                return new PossibleOrder(Numbers.ToList());
            }

        }
    }
}
