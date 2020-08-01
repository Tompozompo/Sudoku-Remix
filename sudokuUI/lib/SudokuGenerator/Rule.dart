import 'SudokuCell.dart';

abstract class Rule {
  Iterable<SudokuCell> cells;
  Iterable<int> possibleValues;

  Rule(this.cells, this.possibleValues);

  bool isSatisfied();

  List<int> getOptions(SudokuCell cell);
}