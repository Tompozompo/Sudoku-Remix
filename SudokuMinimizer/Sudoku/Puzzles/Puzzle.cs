using System;
using System.Collections.Generic;
using System.Linq;

using Sudoku.Cells;
using Sudoku.Rules;
using SudokuMinimizer;

namespace Sudoku.Puzzles
{

    public enum DifficultyOption {
        Easy, 
        Medium, 
        Hard,
        Expert, 
        Master
    }

    public abstract class Puzzle : IEquatable<Puzzle>
    {
        private readonly IList<SudokuCell> cells;

        protected Puzzle(int size, int internalSize)
        {
            Size = size;
            InternalSize = internalSize;
            PossibleValues = Enumerable.Range(1, Size);

            // Init the cells
            cells = Enumerable.Range(0, Size * Size).Select(i => new SudokuCell(i / Size, i % Size, this)).ToList();
        }

        #region Properties

        public int Size { get; private set; }

        public int InternalSize { get; private set; }

        public IEnumerable<int> PossibleValues { get; private set; }

        public IList<Rule> PuzzleRules { get; protected set; } = new List<Rule>();

        private DifficultyOption _difficulty;
        public DifficultyOption Difficulty 
        {
            get
            {
                return _difficulty;
            }
            set
            {
                _difficulty = value;
                ComputeOptions();
            }
        }

        public int Count
        {
            get
            {
                return GetAllCells().Count(x => x.Value != null);
            }
        }

        public SudokuCell this[int x, int y]
        {
            get
            {
                int index = x * Size + y;
                return cells[index];
            }
        }

        #endregion Properties

        #region Enumerators

        public IList<SudokuCell> GetAllCells()
        {
            return cells;
        }

        public IList<SudokuCell> GetCol(int columnNumber)
        {
            return Enumerable.Range(0, Size)
                    .Select(x => this[x, columnNumber]).ToList();
        }

        public IList<SudokuCell> GetRow(int rowNumber)
        {
            return Enumerable.Range(0, Size)
                    .Select(x => this[rowNumber, x]).ToList();
        }

        public IList<SudokuCell> GetBox(int rowNumber, int columnNumber)
        {
            return Enumerable.Range(0, Size)
                    .Select(x => this[rowNumber * InternalSize + x / InternalSize, columnNumber * InternalSize + x % InternalSize]).ToList();
        }

        public IList<SudokuCell> GetDiagD()
        {
            return Enumerable.Range(0, Size)
                    .Select(x => this[x, x]).ToList();
        }

        public IList<SudokuCell> GetDiagU()
        {
            return Enumerable.Range(0, Size)
                    .Select(x => this[x, Size - x - 1]).ToList();
        }

        #endregion Enumerators

        #region State Checkers

        // Checks that all rules are satisfied.
        public bool IsValid()
        {
            return PuzzleRules.All(x => x.IsSatisfied());
        }

        // Checks that all cells are non-null, and the puzzle is valid.
        public bool IsComplete()
        {
            return GetAllCells().All(x => x.Value != null) && IsValid();
        }

        // Checks that all null cells have at least 1 option
        public bool IsPossible()
        {
            return GetAllCells().Where(x => x.Value == null).All(x => x.Options.Any());
        }

        public bool IsSquare()
        {
            return true;
        }

        #endregion State Checkers

        #region Cloneable

        protected abstract Puzzle GetInstance();

        public virtual Puzzle Clone()
        {
            // Setup the clone puzzle, will init all cells and rules
            Puzzle p = GetInstance();
            p._difficulty = _difficulty;

            // Copy cell data
            foreach(var cell in p.cells)
            {
                cell.CopyFrom(this[cell.Row, cell.Col]);
            }

            return p;
        }

        #endregion Abstracts

        #region Functions

        // Most option strategies adapted from http://www.sudokuoftheday.com/techniques/
        public void ComputeOptions()   
        {
            // Compute the base options for each cell, based on its individual rules and clue values
            foreach (var cell in GetAllCells())
            {
                cell.ComputeOptions();
            }

            if (Difficulty == DifficultyOption.Easy)
                return;

            int counter = 0;
            while (counter < 2) // allow multiple passes, since new information from later tests could reveal more things in the first tests
            {
                // Reduce options using medium techniques

                // Hidden Singles
                foreach (var cell in GetAllCells().Where(x => x.Options.Count > 1))
                {
                    foreach (var rule in cell.Rules.Where(x => x is UniqueRule))
                    {
                        var allClueOptions = rule.Cells.Where(x => x != cell && x.Value == null).Select(x => x.Options);

                        // Hidden Single (there is a value confined to a single cell of a rule)
                        var clueOptions = allClueOptions.SelectMany(x => x).Distinct();
                        var onlyOptions = cell.Options.Except(clueOptions).ToList();
                        if (onlyOptions.Count == 1) // onlyOptions.Take(2).Count() == 1
                        {
                            cell.Options = onlyOptions;
                        }
                    }
                }

                // Candidate Lines (within a box, if some option only exists on a row/col, it must exist in that box in that row/col, so the rest of that row/col can be unmarked)
                for (int r = 0; r < InternalSize; r++)
                {
                    for (int c = 0; r < InternalSize; r++)
                    {
                        var box = GetBox(r, c).ToList();
                        for (int i = 0; i < InternalSize; i++) //internal rows
                        {
                            var row = Enumerable.Range(0, InternalSize).Select(x => box[i * InternalSize + x]);
                            var rowOptions = row.SelectMany(x => x.Options);
                            var others = box.Except(row);
                            var othersOptions = others.SelectMany(x => x.Options);
                            var diff = rowOptions.Except(othersOptions);
                            if (diff.Any())
                            {
                                int index = row.Select(x => x.Row).First();
                                var bigRow = GetRow(index).Except(row);
                                foreach (var bigCell in bigRow.Where(x => x.Value != null))
                                {
                                    bigCell.Options = bigCell.Options.Except(diff).ToList();
                                }
                            }
                        }
                        for (int j = 0; j < InternalSize; j++) //internal cols 
                        {
                            var col = Enumerable.Range(0, InternalSize).Select(x => box[j + x * InternalSize]);
                            var colOptions = col.SelectMany(x => x.Options);
                            var others = box.Except(col);
                            var othersOptions = others.SelectMany(x => x.Options);
                            var diff = colOptions.Except(othersOptions);
                            if (diff.Any())
                            {
                                int index = col.Select(x => x.Col).First();
                                var bigCol = GetCol(index).Except(col);
                                foreach (var bigCell in bigCol.Where(x => x.Value != null))
                                {
                                    bigCell.Options = bigCell.Options.Except(diff).ToList();
                                }
                            }
                        }
                    }
                }

                // Multiple Lines/Double Pair (if a value is restricted to 2 rows/col in 2 boxes, then it must be in the remaining row/col of the remaining box)
                for (int r = 0; r < InternalSize; r++)
                {
                    for (int c = 0; r < InternalSize; r++)
                    {
                        var box = GetBox(r, c).ToList();
                        for (int i = 0; i < InternalSize; i++) // rows
                        {
                            var row = Enumerable.Range(0, InternalSize).Select(x => box[i * InternalSize + x]);
                            var rowOptions = row.SelectMany(x => x.Options);
                            int index = row.Select(x => x.Row).First();
                            var bigRow = GetRow(index).Except(row);
                            var bigRowOptions = bigRow.SelectMany(x => x.Options);
                            var diff = rowOptions.Except(bigRowOptions);
                            if (diff.Any())
                            {
                                var others = box.Except(row);
                                foreach (var other in others.Where(x => x.Value != null))
                                {
                                    other.Options = other.Options.Except(diff).ToList();
                                }
                            }
                        }
                        for (int i = 0; i < InternalSize; i++) // cols
                        {
                            var col = Enumerable.Range(0, InternalSize).Select(x => box[i * InternalSize + x]);
                            var colOptions = col.SelectMany(x => x.Options);
                            int index = col.Select(x => x.Row).First();
                            var bigCol = GetCol(index).Except(col);
                            var bigColOptions = bigCol.SelectMany(x => x.Options);
                            var diff = colOptions.Except(bigColOptions);
                            if (diff.Any())
                            {
                                var others = box.Except(col);
                                foreach (var other in others.Where(x => x.Value != null))
                                {
                                    other.Options = other.Options.Except(diff).ToList();
                                }
                            }
                        }
                    }
                }

                if (Difficulty == DifficultyOption.Medium)
                    return;

                // Reduce options using advanced techniques

                int limit = Difficulty == DifficultyOption.Hard ? 2 : 3;
                // Naked subsets (n cells only contain n possible options, then this cell may not contain any of those n options)
                foreach (var cell in GetAllCells().Where(x => x.Options.Skip(1).Any())) //  x.Options.Count() > 1
                {
                    foreach (var rule in cell.Rules.Where(x => x is UniqueRule))
                    {
                        if (!cell.Options.Skip(1).Any()) break; // only 1 option, can stop
                        var allClueOptions = rule.Cells.Where(x => x != cell && x.Value == null).Select(x => x.Options);
                        var clueOptionsSubSets = allClueOptions.SubSetsOf();

                        for (int subsetSize = 2; subsetSize <= limit; subsetSize++)
                        {
                            var subSets = clueOptionsSubSets.Where(x => x.Count() == subsetSize);
                            foreach (var subset in subSets)
                            {
                                var options = subset.SelectMany(x => x).Distinct();
                                if (options.Count() == subsetSize)
                                {
                                    cell.Options = cell.Options.Except(options).ToList();
                                }
                            }
                        }

                    }
                }

                // Hidden subsets (n options are confined to n spaces, those spaces must contain those n options)
                foreach (var cell in GetAllCells().Where(x => x.Options.Skip(1).Any())) //  x.Options.Count() > 1
                {
                    foreach (var rule in cell.Rules.Where(x => x is UniqueRule))
                    {
                        if (!cell.Options.Skip(1).Any()) break; // only 1 option, can stop
                        var allClueOptions = rule.Cells.Where(x => x != cell && x.Value == null).Select(x => x.Options);
                        var allClueOptionsCount = allClueOptions.Count();
                        var clueOptionsSubSets = allClueOptions.SubSetsOf();
                        for (int subsetSize = allClueOptionsCount - 1; (subsetSize >= allClueOptionsCount - limit + 1) && subsetSize > 0; subsetSize--)
                        {
                            var subSets = clueOptionsSubSets.Where(x => x.Count() == subsetSize);
                            foreach (var subset in subSets)
                            {
                                var subsetOptions = subset.SelectMany(x => x).Distinct();
                                var remainingOptions = cell.Options.Except(subsetOptions).ToList();
                                if (remainingOptions.Count == allClueOptionsCount - subsetSize + 1)
                                {
                                    cell.Options = remainingOptions;
                                }
                            }
                        }
                    }
                }

                if (Difficulty == DifficultyOption.Hard)
                    return;
                
                // Reduce options using expert techniques

                // Just do everything again
                counter++;
            }
        }

        #endregion Functions

        #region IEquatable

        public bool Equals(Puzzle other)
        {
            return cells.All(x => x.Equals(other[x.Row, x.Col]));
        }

        #endregion IEquatable
    }
}