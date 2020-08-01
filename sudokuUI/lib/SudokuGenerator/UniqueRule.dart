import 'Rule.dart';
import 'SudokuCell.dart';

class UniqueRule extends Rule {
  UniqueRule(Iterable<SudokuCell> cells, Iterable<int> possibleValues) : super(cells, possibleValues);

  @override
  bool isSatisfied() {
    Set<int> foundValues = new Set<int>();
    for (var cell in cells) {
      if (cell.value == null) { // skip null
        continue;
      }
      else if (!possibleValues.contains(cell.value)) { // invalid value
        return false;
      }
      else if (foundValues.contains(cell.value)) { // repeat value
        return false;
      }
      foundValues.add(cell.value);
    }
    return true;
  }

  @override
  List<int> getOptions(SudokuCell cell) {
    if (!cells.contains(cell)) {
      throw new Exception();
    }
    else if (cell.value == null) {
      // Can't be any values that exist in another cell
      return possibleValues.toSet().difference(cells.map((x) => x.value ?? 0).toSet()).toList();
    }
    else {
      return new List<int>();
    }
  }
}