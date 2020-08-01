using System;
using System.Collections.Generic;
using System.Linq;

using Sudoku.Puzzles;

namespace SudokuMinimizer
{
    class OptionsCmdDisplayStrategy : IDisplayStrategy
    {
        public void DisplayImpl(Puzzle p)
        {
            int internal_count = p.Size / p.InternalSize;
            for (int i = 0; i < p.Size; i++)
            {
                if (i % p.InternalSize == 0)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                }
                var row = p.GetRow(i);
                IList<int> values = p.PossibleValues.ToList();

                for (int j = 0; j < p.Size / p.InternalSize; j++)
                {
                    int count = 0;
                    foreach (var r in row)
                    {
                        if (count % 3 == 0)
                        {
                            Console.Write("| ");
                        }
                        int index = j * internal_count;
                        Console.Write(string.Format("{0,1} {1,1} {2,1} |", 
                            r.Value != null ? r.Value : r.Options.Contains(values[index]) ? (int?)values[index] : null, 
                            r.Value != null ? r.Value : r.Options.Contains(values[index + 1]) ? (int?)values[index + 1] : null, 
                            r.Value !=  null ? r.Value : r.Options.Contains(values[index + 2]) ? (int?)values[index + 2] : null));
                        count++;
                    }
                    Console.Write("|");
                    Console.WriteLine();
                }
                Console.WriteLine("----------------------------------------------------------------------");
            }
        }
    }
}
