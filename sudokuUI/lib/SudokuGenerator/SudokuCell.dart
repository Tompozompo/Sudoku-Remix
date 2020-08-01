import 'Rule.dart';
import 'SudokuPuzzle.dart';

class SudokuCell {
  int _value;
  int _row;
  int _col;
  SudokuPuzzle _parent;

  SudokuCell(this._row, this._col, this._parent);

  int get row {
    return _row;
  }

  int get col {
    return _col;
  }

  int get value {
    return _value;
  }

  set value(int val) {
    _value = val;
    //_parent.computeOptions();
  }

  List<int> options;

  List<Rule> rules = new List<Rule>();

  void computeOptions() {
    options = _parent.possibleValues;
    rules.forEach((r) {
      options = r.getOptions(this).toSet().intersection(options.toSet()).toList();
    });
  }

  int getHashCode() {
    return row + col + (value ?? 0);
  }

  void copyFrom(SudokuCell cell) {
    _value = cell.value;
    options = List.from(cell.options);
  }

  String toString() {
    return value.toString();
  }

  bool equals(SudokuCell other) {
    return other != null && other.row == row && other.col == col && other.value == value;
  }
}