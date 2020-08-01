using Sudoku.Rules;

namespace Sudoku.Puzzles
{
    public class DiagonalSumPuzzle : ClassicPuzzle
    {
        private readonly int sum = 30;

        public DiagonalSumPuzzle(int size, int internalSize) : base(size, internalSize)
        {
            SetRules();
        }

        protected override Puzzle GetInstance()
        {
            return new DiagonalSumPuzzle(Size, InternalSize);
        }

        private void SetRules()
        {
            // Diagonals
            var diagD = GetDiagD();
            Rule ruleD = new SumRule(diagD, PossibleValues, sum);
            PuzzleRules.Add(ruleD);
            foreach (var c in diagD)
            {
                c.Rules.Add(ruleD);
            }

            var diagU = GetDiagU();
            Rule ruleU = new SumRule(diagU, PossibleValues, sum);
            PuzzleRules.Add(ruleU);
            foreach (var c in diagU)
            {
                c.Rules.Add(ruleU);
            }
        }
    }
}

