import 'package:sudoku/SudokuGenerator/DifficultyOption.dart';

import 'RecursiveSolver.dart';
import 'SudokuPuzzle.dart';

class PuzzleFactory {

  static SudokuPuzzle getEmptyPuzzle() {
    return new SudokuPuzzle();
  }

  static SudokuPuzzle getRandomSolution() {
    SudokuPuzzle p = getEmptyPuzzle();
    p.difficulty = DifficultyOption.Easy;
    p.computeOptions();
    p = RecursiveSolver.getRandomSolution(p);
    if (p == null || !p.isComplete()) {
      throw new Exception();
    }
    return p;
  }
}
