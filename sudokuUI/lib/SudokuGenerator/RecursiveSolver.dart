import 'package:sudoku/SudokuGenerator/DifficultyOption.dart';

import 'SudokuCell.dart';
import 'SudokuPuzzle.dart';

class Counter {
  int count = 0;

  int increment() => count++;
}

class RecursiveSolver {
  static bool uniqueSolution(SudokuPuzzle p, {bool quick = true}) {
    SudokuPuzzle clone = p.clone();
    clone.difficulty = DifficultyOption.Expert;
    Counter counter = new Counter();
    return uniqueSolutionRecursion(clone, quick, counter);
  }

  static bool uniqueSolutionRecursion(SudokuPuzzle p, bool quick, Counter counter) {
    if (counter.count > 1) { // too many solutions, immediately end
      return false;
    }

    SudokuPuzzle clone = p.clone();
    SudokuCell c = getFirstNull(clone);
    if (c == null) { // no nulls remain, check if this is a solution
      if (clone.isComplete()) {
        counter.increment();
      }
    }
    else {
      List<int> options = quick ? c.options : p.possibleValues;
      for (int o in options) {
        c.value = o;
        if (clone.isPossible() && (quick || clone.isValid())) { // the quick version should not choose invalid puzzles
          uniqueSolutionRecursion(clone, quick, counter);
          if (counter.count > 1) { // too many solutions, immediately end
            return false;
          }
        }
        c.value = null;
      }
    }

    if (counter.count == 1) { //only 1 solution!
      return true;
    }
    else {
      return false;
    }
  }

  static SudokuPuzzle getRandomSolution(SudokuPuzzle p, {bool quick = true}) {
    SudokuPuzzle clone = p.clone();
    SudokuCell c = getFirstNull(clone);
    if (c == null) { // no nulls remain, check if this is a solution
      if (clone.isComplete()) {
        return clone;
      }
    }
    else {
      List<int> options = quick ? c.options : p.possibleValues;
      options.shuffle();
      for (int o in options) {
        c.value = o;
        if (clone.isPossible() && (quick || clone.isValid())) { // the quick version should not choose invalid puzzles
          var result = getRandomSolution(clone, quick: quick);
          if (result != null) { // Found a solution!
            return result;
          }
        }
        c.value = null;
      }
    }
    return null; // no solution found
  }

  static SudokuCell getFirstNull(SudokuPuzzle p) {
    for (int i = 0; i < p.size; i++) {
      var row = p.getRow(i);
      for (var r in row) {
        if (r.value == null)
          return r;
      }
    }
    return null;
  }
}