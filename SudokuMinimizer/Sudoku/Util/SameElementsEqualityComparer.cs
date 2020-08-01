using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Util
{
    class SameElementsEqualityComparer<T> : IEqualityComparer<IList<T>>
    {
        public bool Equals(IList<T> x, IList<T> y)
        {
            return (x.Count == y.Count) && !x.Except(y).Any();
        }

        public int GetHashCode(IList<T> obj)
        {
            return obj.Count;
        }
    }
}
