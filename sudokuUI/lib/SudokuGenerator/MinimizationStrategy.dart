import 'package:flutter/cupertino.dart';

import 'RecursiveSolver.dart';
import 'SudokuCell.dart';
import 'SudokuPuzzle.dart';

class MinimizationStrategy {
  static SudokuPuzzle oneOptionMinimize(SudokuPuzzle puzzle, {int minSize = 0}) {
    if (!RecursiveSolver.uniqueSolution(puzzle)) { // if the input doesn't have a unique solution, return null
      return null;
    }

    SudokuPuzzle p = puzzle.clone();
    bool uniqueSolution = false;
    while (!uniqueSolution) {
      p = puzzle.clone();
      List<SudokuCell> cells = p.getAllCells().where((x) => x.value != null).toList();
      cells.shuffle();
      for (SudokuCell c in cells) {
        if (p.count <= minSize) {
          break;
        }
        int val = c.value;
        c.value = null;
        if (c.options.length != 1) {
          c.value = val;
        }
      }
      uniqueSolution = RecursiveSolver.uniqueSolution(p); // If there is not a unique solution after all this, just restart
    }
    return p;
  }

  static SudokuPuzzle uniqueSolutionMinimize(SudokuPuzzle puzzle, {int minSize = 0}) {
    if (!RecursiveSolver.uniqueSolution(puzzle)) { // if the input doesn't have a unique solution, return null
      return null;
    }

    SudokuPuzzle p = puzzle.clone();
    var allCells = p.getAllCells().where((x) => x.value != null).toList();
    allCells.shuffle();
    int counter = 0;
    for (SudokuCell c in allCells) { // Attempt to remove each cell, making sure that a unique solution remains
      if (p.count <= minSize) {
        break;
      }
      debugPrint(counter.toString() + " / " + allCells.length.toString());
      int val = c.value;
      c.value = null;
      bool unique = p.getAllCells().any((x) => x.options.length == 1) && RecursiveSolver.uniqueSolution(p);
      //bool unique = RecursiveSolver.uniqueSolution(p);
      if (!unique) {
        c.value = val;
      }
      counter++;
    }
    return p;
  }
}