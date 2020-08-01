import 'package:sudoku/SudokuGenerator/DifficultyOption.dart';

import 'Rule.dart';
import 'SudokuCell.dart';
import 'UniqueRule.dart';
import 'ExtensionMethods.dart';


class SudokuPuzzle {
  List<SudokuCell> _cells;

  SudokuPuzzle() {
    _cells = List.generate(_size * _size, (i) => SudokuCell(i ~/ _size, i % _size, this));

    List<Rule> rules = new List<Rule>();
    // Rows
    for (int i = 0; i < _size; i++) {
      var row = getRow(i);
      Rule rule = new UniqueRule(row, possibleValues);
      rules.add(rule);
      for (var r in row) {
        r.rules.add(rule);
      }
    }

    // Columns
    for (int i = 0; i < _size; i++) {
      var col = getCol(i);
      Rule rule = new UniqueRule(col, possibleValues);
      rules.add(rule);
      for (var c in col) {
        c.rules.add(rule);
      }
    }

    // 3x3 cells
    for (int i = 0; i < _internalSize; i++) {
      for (int j = 0; j < _internalSize; j++) {
        var box = getBox(i, j);
        Rule rule = new UniqueRule(box, possibleValues);
        rules.add(rule);
        for (var b in box) {
          b.rules.add(rule);
        }
      }
    }

    puzzleRules.toList().addAll(rules);
  }

  //#region Properties

  Iterable<int> _possibleValues = List.generate(_size, (i) => i+1);
  Iterable<int> get possibleValues {
    return _possibleValues;
  }

  Iterable<Rule> puzzleRules = new List<Rule>();

  static int _size = 9;
  int get size {
    return _size;
  }

  static int _internalSize = 3;
  int get internalSize {
    return _internalSize;
  }

  DifficultyOption _difficulty;
  DifficultyOption get difficulty {
    return _difficulty;
  }

  set difficulty(DifficultyOption value) {
    _difficulty = value;
    computeOptions();
  }

  int get count {
    return _cells.where((x) => x.value != null).length;
  }

  SudokuCell getCell(int row, int col) {
    int index = row*_size + col;
    return _cells[index];
  }

  //#endregion Properties

  //#region Enumerators

  Iterable<SudokuCell> getCol(int columnNumber) {
    return Iterable.generate(_size, (x) => getCell(x, columnNumber));
  }

  Iterable<SudokuCell> getRow(int rowNumber) {
    return Iterable.generate(_size, (x) => getCell(rowNumber, x));
  }

  Iterable<SudokuCell> getBox(int rowNumber, int columnNumber) {
    return Iterable.generate(_size, (x) => getCell(rowNumber * _internalSize + x ~/ _internalSize, columnNumber * _internalSize + x % _internalSize));
  }

  Iterable<SudokuCell> getDiagonalD() {
    return Iterable.generate(_size, (x) => getCell(x, x));
  }

  Iterable<SudokuCell> getDiagonalU() {
    return Iterable.generate(_size, (x) => getCell(x, _size - x - 1));
  }

  Iterable<SudokuCell> getAllCells() {
    return _cells;
  }

  //#endregion Enumerators

  //#region State Checkers

  bool isValid()
  {
    return puzzleRules.every((x) => x.isSatisfied());
  }

  // Checks that all cells are non-null, and the puzzle is valid.
  bool isComplete()
  {
    return _cells.every((x) => x.value != null) && isValid();
  }

  // Checks that all null cells have at least 1 option
  bool isPossible()
  {
    return _cells.where((x) => x.value == null).every((x) => x.options.length != 0);
  }

  //#endregion State Checkers

  //#region Cloneable

  SudokuPuzzle clone() {
    SudokuPuzzle p = new SudokuPuzzle();
    p._difficulty = _difficulty;
    p._cells.forEach((cell) => cell.copyFrom(getCell(cell.row, cell.col)));
    return p;
  }

  //#endregion Cloneable

  //#region Functions

  void computeOptions() {
    // Compute the base options, based on the rules for each cell
    _cells.forEach((cell) => cell.computeOptions());

    if (difficulty == DifficultyOption.Easy)
      return;

    int counter = 0;
    while (counter < 2) // allow multiple passes, since new information from later tests could reveal more things in the first tests
    {
      // Reduce options using medium techniques

      // Hidden Singles
      for (var cell in _cells.where((x) => x.options.length > 1)) {
        for (var rule in cell.rules.where((x) => x is UniqueRule)) {
          var allClueOptions = rule.cells.where((x) => x != cell && x.value == null).map((x) => x.options);

          // Hidden Single
          var clueOptions = allClueOptions.expand((x) => x).toSet();
          var onlyOptions = cell.options.toSet().difference(clueOptions);
          if (onlyOptions.length == 1) {
            cell.options = onlyOptions.toList();
          }
        }
      }

      // Candidate Lines (within a box, if some option only exists on a row/col, it must exist in that box in that row/col, so the rest of that row/col can be unmarked)
      for (int r = 0; r < internalSize; r++) {
        for (int c = 0; r < internalSize; r++) {
          var box = getBox(r, c).toList();
          for (int i = 0; i < internalSize; i++) { //internal rows
            var row = List.generate(internalSize, (x) => box[i * internalSize + x]).toSet();
            var rowOptions = row.expand((x) => x.options);
            var others = box.toSet().difference(row);
            var othersOptions = others.expand((x) => x.options);
            var diff = rowOptions.toSet().difference(othersOptions.toSet());
            if (diff.length != 0) {
              int index = row
                  .map((x) => x.row)
                  .first;
              var bigRow = getRow(index).toSet().difference(row);
              for (var bigCell in bigRow.where((x) => x.value != null)) {
                bigCell.options = bigCell.options.toSet().difference(diff).toList();
              }
            }
          }
          for (int i = 0; i < internalSize; i++) { //internal cols
            var col = List.generate(internalSize, (x) => box[i + x * internalSize]).toSet();
            var colOptions = col.expand((x) => x.options);
            var others = box.toSet().difference(col);
            var othersOptions = others.expand((x) => x.options);
            var diff = colOptions.toSet().difference(othersOptions.toSet());
            if (diff.length != 0) {
              int index = col
                  .map((x) => x.col)
                  .first;
              var bigCol = getCol(index).toSet().difference(col);
              for (var bigCell in bigCol.where((x) => x.value != null)) {
                bigCell.options = bigCell.options.toSet().difference(diff).toList();
              }
            }
          }
        }
      }

      // Multiple Lines/Double Pair (if a value is restricted to 2 rows/col in 2 boxes, then it must be in the remaining row/col of the remaining box)
      for (int r = 0; r < internalSize; r++) {
        for (int c = 0; r < internalSize; r++) {
          var box = getBox(r, c).toList();
          for (int i = 0; i < internalSize; i++) { // rows
            var row = List.generate(internalSize, (x) => box[i * internalSize + x]).toSet();
            var rowOptions = row.expand((x) => x.options);
            int index = row
                .map((x) => x.row)
                .first;
            var bigRow = getRow(index).toSet().difference(row);
            var bigRowOptions = bigRow.expand((x) => x.options);
            var diff = rowOptions.toSet().difference(bigRowOptions.toSet());
            if (diff.length != 0) {
              var others = box.toSet().difference(row);
              for (var other in others.where((x) => x.value != null)) {
                other.options = other.options.toSet().difference(diff).toList();
              }
            }
          }
          for (int i = 0; i < internalSize; i++) { // cols
            var col = List.generate(internalSize, (x) => box[i * internalSize + x]).toSet();
            var colOptions = col.expand((x) => x.options);
            int index = col
                .map((x) => x.row)
                .first;
            var bigCol = getCol(index).toSet().difference(col);
            var bigColOptions = bigCol.expand((x) => x.options);
            var diff = colOptions.toSet().difference(bigColOptions.toSet());
            if (diff.length != 0) {
              var others = box.toSet().difference(col);
              for (var other in others.where((x) => x.value != null)) {
                other.options = other.options.toSet().difference(diff).toList();
              }
            }
          }
        }
      }


    if (difficulty == DifficultyOption.Medium)
        return;

      // Reduce options using advanced techniques

      int limit = 3;
      // Naked subsets (n cells only contain n possible options, then this cell may not contain any of those n options)
      for (var cell in _cells.where((x) => x.options.length > 1)) {
        for (var rule in cell.rules.where((x) => x is UniqueRule)) {
          var allClueOptions = rule.cells.where((x) => x != cell && x.value == null).map((x) => x.options);
          var clueOptionsSubSets = allClueOptions.subSetsOf();
          for (int subsetSize = 2; subsetSize <= limit; subsetSize++) {
            var subSets = clueOptionsSubSets.where((x) => x.length == subsetSize).map((x) => x);
            for (var subset in subSets) {
              var options = subset.expand((x) => x).toSet();
              if (options.length == subsetSize) {
                cell.options = cell.options.toSet().difference(options).toList();
              }
            }
          }
        }
      }

      // Hidden subsets (n options are confined to n spaces, those spaces must contain those n options)
      for (var cell in _cells.where((x) => x.options.length > 1)) {
        for (var rule in cell.rules.where((x) => x is UniqueRule)) {
          var allClueOptions = rule.cells.where((x) => x != cell && x.value == null).map((x) => x.options);
          var clueOptionsSubSets = allClueOptions.subSetsOf();
          for (int subsetSize = allClueOptions.length - 1; (subsetSize >= allClueOptions.length - limit + 1) && subsetSize > 0; subsetSize--) {
            var subSets = clueOptionsSubSets.where((x) => x.length == subsetSize).map((x) => x);
            for (var subset in subSets) {
              var subsetOptions = subset.expand((x) => x).toSet();
              var subsetOnlyOptions = cell.options.toSet().difference(subsetOptions);
              if (subsetOnlyOptions.length == allClueOptions.length - subsetSize + 1) {
                cell.options = subsetOnlyOptions.toList();
              }
            }
          }
        }
      }

      if (difficulty == DifficultyOption.Hard)
        return;

      // Reduce options using advanced techniques

      // Just do everything again
      counter++;
    }
  }

  //#endregion Functions

  //#region IEquatable

  bool equals(SudokuPuzzle other) {
    return _cells.every((cell) => cell.equals(other.getCell(cell.row, cell.col)));
  }

  //#endregion IEquatable

}