import 'dart:collection';
import 'dart:io';
import 'dart:typed_data';
import 'dart:async' show Future, Timer;

import 'package:flutter/widgets.dart';
import 'package:sqflite/sqflite.dart';
import 'package:sudoku/SudokuGenerator/DifficultyOption.dart';
import 'package:path/path.dart';
import 'package:flutter/services.dart';

import 'SudokuGenerator/SudokuPuzzle.dart';

enum InsertMode {
  Normal,
  Pencil,
  Eraser
}

class BoardState {
  SudokuPuzzle puzzle;
  List<List<List<int>>> marks;

  BoardState(this.puzzle, this.marks);
}

class SudokuModel extends ChangeNotifier {
  SudokuPuzzle puzzle;
  SudokuPuzzle solution;
  List<List<bool>> given;
  List<List<List<int>>> marks;

  int selectedNumber;
  int selectedCellR;
  int selectedCellC;
  List<bool> mode;

  Queue<BoardState> undoQueue;
  Queue<BoardState> redoQueue;

  Stopwatch timer = new Stopwatch();

  SudokuModel() {
    timer.start();
    Timer.periodic(
      Duration(seconds: 1),
      (timer) => notifyListeners()
    );
  }

  init() {
    marks = List.generate(9, (_) => List.generate(9, (_) => new List<int>()));

    selectedNumber = -1;
    selectedCellR = -1;
    selectedCellC = -1;
    mode = List.generate(3, (index) => index == 0 ? true : false);

    undoQueue = new Queue<BoardState>();
    redoQueue = new Queue<BoardState>();

    timer.reset();
  }

  List<int> get numberCounts {
    return List.generate(9, (index) => puzzle.getAllCells().where((x) => x.value == index+1).length);
  }


  List<bool> get enabledKeyPadCell {
    return List.generate(9, (index) => puzzle.getAllCells().where((x) => x.value == index+1).length < 9);
}

  int getValue(int r, int c) {
    return puzzle.getCell(r, c).value;
  }

  void setValue(int r, int c, int v) {
    debugPrint("setting value $r, $c = $v");
    var cell = puzzle.getCell(r, c);
    if(!given[r][c] && cell.value != v) {
      addToQueue(undoQueue);
      redoQueue.clear();
      cell.value = v;
      notifyListeners();
    }
  }

  void addPencil(int r, int c, int v) {
    debugPrint("adding pencil $r, $c = $v");
    if(!given[r][c] && !marks[r][c].contains(v)) {
      addToQueue(undoQueue);
      redoQueue.clear();
      marks[r][c].add(v);
      notifyListeners();
    }
  }

  void removePencil(int r, int c, int v) {
    debugPrint("removing pencil $r, $c = $v");
    if(!given[r][c] && marks[r][c].contains(v)) {
      addToQueue(undoQueue);
      redoQueue.clear();
      marks[r][c].remove(v);
      notifyListeners();
    }
  }

  void remove(int r, int c) {
    debugPrint("removing $r, $c");
    var cell = puzzle.getCell(r, c);
    if (!given[r][c] && (marks[r][c].isNotEmpty || cell.value != 0)) {
      addToQueue(undoQueue);
      redoQueue.clear();
      cell.value = 0;
      marks[r][c].clear();
      notifyListeners();
    }
  }

  void setMode(int index) {
    for (int i = 0; i < mode.length; i++) {
      mode[i] = (i == index);
    }
    notifyListeners();
  }

  void setSelectedNumber(int index) {
    selectedNumber = selectedNumber == index ? -1 : index;
    notifyListeners();
  }

  void setSelectedCell(int r, int c) {
    if(selectedCellR == r && selectedCellC == c) {
      selectedCellC = selectedCellR = -1;
    } else {
      selectedCellR = r;
      selectedCellC = c;
    }
    notifyListeners();
  }

  InsertMode getMode() {
    switch(mode.indexOf(true)){
      case 0:
        return InsertMode.Normal;
      case 1:
        return InsertMode.Pencil;
      case 2:
        return InsertMode.Eraser;
      default:
        return null;
    }
  }

  void undo() {
    debugPrint("undo");
    if(undoQueue.isNotEmpty) {
      addToQueue(redoQueue);
      BoardState boardState = undoQueue.removeFirst();
      puzzle = boardState.puzzle;
      marks = boardState.marks;
      notifyListeners();
    }
  }

  void redo() {
    debugPrint("redo");
    if(redoQueue.isNotEmpty) {
      addToQueue(undoQueue);
      BoardState boardState = redoQueue.removeFirst();
      puzzle = boardState.puzzle;
      marks = boardState.marks;
      notifyListeners();
    }
  }

  void restart() {
    init();

    for(int r = 0; r < 9; r++) {
      for(int c = 0; c < 9; c++) {
        if(!given[r][c]) {
          puzzle.getCell(r, c).value = null;
        }
      }
    }

    notifyListeners();
  }

  void showSolution() {
    puzzle = solution.clone();
    notifyListeners();
  }

  void showOptions(DifficultyOption diff) {
    debugPrint("filling marks automatically, diff $diff");
    DifficultyOption temp = puzzle.difficulty;
    puzzle.difficulty = diff;
    puzzle.computeOptions();
    for(int i = 0; i < 9; i++) {
      for(int j = 0; j < 9; j++) {
        marks[i][j] = puzzle.getCell(i, j).options.toList();
      }
    }
    puzzle.difficulty = temp;
    notifyListeners();
  }

  bool validate() {
    return puzzle.isValid();
  }

  void addToQueue(Queue<BoardState> queue) {
    List<List<List<int>>> queueMarks = List.generate(9, (_) => List.generate(9, (_) => new List<int>()));
    for(int i = 0; i < 9; i++) {
      for (int j = 0; j < 9; j++) {
        queueMarks[i][j] = List<int>.from(marks[i][j]);
      }
    }
    SudokuPuzzle queuePuzzle = puzzle.clone();
    queue.addFirst(new BoardState(queuePuzzle, queueMarks));
  }

  Future<int> randomPuzzle(DifficultyOption difficulty) async {
    init();
    given = List.generate(9, (_) => List.generate(9, (_) => false));

    var databasesPath = await getDatabasesPath();
    var path = join(databasesPath, "puzzles.db");

    var exists = await databaseExists(path);
    if (!exists) {
      print("Creating new copy from asset");
      try {
        await Directory(dirname(path)).create(recursive: true);
      } catch (_) {}
      ByteData data = await rootBundle.load(join("assets", "puzzles.db"));
      List<int> bytes = data.buffer.asUint8List(data.offsetInBytes, data.lengthInBytes);
      await File(path).writeAsBytes(bytes, flush: true);
    } else {
      print("Opening existing database");
    }

    // open the database
    var db = await openDatabase(path, readOnly: true);
    String query = '''
    SELECT id, puzzle, solution
    FROM puzzles 
    WHERE id >= (abs(random()) % (SELECT max(id) FROM puzzles))
    LIMIT 1;
    ''';
    var dbResult = (await db.rawQuery(query)).first;
    print(dbResult.toString());
    solution = deserialize(dbResult["solution"]);
    puzzle = deserialize(dbResult["puzzle"]);
    debugPrint("count: ${puzzle.count}");
    debugPrint("difficulty: ${puzzle.difficulty}");
    for(int r = 0; r < 9; r++) {
      for(int c = 0; c < 9; c++) {
        given[r][c] = puzzle.getCell(r, c).value != null;
      }
    }
    return 0;
  }

  SudokuPuzzle deserialize(String serial) {
    SudokuPuzzle puzzle = new SudokuPuzzle();
    puzzle.difficulty = DifficultyOption.Easy;
    int i = 0;
    for(int row = 0; row < 9; row++) {
      for(int col = 0; col < 9; col++) {
        var char = serial[i];
        if(char != '-') {
          int p = int.tryParse(char.toString());
          var cell = puzzle.getCell(row, col);
          cell.value = p;
        }
        i++;
      }
    }
    return puzzle;
  }

  /*
  Future<int> randomPuzzle(DifficultyOption difficulty) async {
    init();
    given = List.generate(9, (_) => List.generate(9, (_) => false));
    solution = PuzzleFactory.getRandomSolution();
    solution.difficulty = difficulty;
    puzzle = MinimizationStrategy.oneOptionMinimize(solution);
    debugPrint("count: ${puzzle.count}");
    if(difficulty == DifficultyOption.Master) puzzle = MinimizationStrategy.uniqueSolutionMinimize(puzzle);
    debugPrint("count: ${puzzle.count}");
    debugPrint("difficulty: ${puzzle.difficulty}");
    for(int r = 0; r < 9; r++) {
      for(int c = 0; c < 9; c++) {
        given[r][c] = puzzle.getCell(r, c).value != null;
      }
    }
    return 0;
  }
  void deserialize() {
    Future<String> future = loadFile('assets/puzzles/KingKnightsPuzzle/24/0.txt');
    future.then((value) => parse(value))
        .catchError((error) => debugPrint(error.toString()));
  }

  Future<String> loadFile(String filePath) async {
    return await rootBundle.loadString(filePath);
  }

  void parse(String serial) {
    List<String> lineSplit =  LineSplitter.split(serial).toList();
    int puzzleEnd = lineSplit.indexOf("");
    int solutionEnd = lineSplit.indexOf('', puzzleEnd+1);
    int ruleEnd = lineSplit.indexOf('', solutionEnd+1);
    lineSplit.sublist(0, puzzleEnd).forEach((line) {
      List<String> split = line.split(',');
      int r = int.parse(split[0]);
      int c = int.parse(split[1]);
      int v = int.parse(split[2]);
      board[r][c] = v;
      given[r][c] = true;
    });
    lineSplit.sublist(puzzleEnd+1, solutionEnd).forEach((line) {
      List<String> split = line.split(',');
      int r = int.parse(split[0]);
      int c = int.parse(split[1]);
      int v = int.parse(split[2]);
      solution[r][c] = v;
    });
    notifyListeners();
  }
  */
}