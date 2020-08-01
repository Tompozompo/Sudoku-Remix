import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:sudoku/ThemeModel.dart';

import 'SudokuModel.dart';

class SudokuBoard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeModel>(
        builder: (context, theme, child) {
          return Table(
            defaultVerticalAlignment: TableCellVerticalAlignment.middle,
            /*
            border: TableBorder(
              horizontalInside: BorderSide(
                  width: 2,
                  color: theme.borderColor,
                  style: BorderStyle.solid
              ),
              verticalInside: BorderSide(
                  width: 2,
                  color: theme.borderColor,
                  style: BorderStyle.solid
              ),
            ),
            */
            border: TableBorder.all(
                width: 2,
                color: theme.borderColor,
                style: BorderStyle.solid
            ),

            children: getSubBoards(),
          );
        }
    );
  }

  List<TableRow> getSubBoards() {
    List<TableRow> subs = new List<TableRow>();
    for (int i = 0; i < 3; i++) {
      TableRow row = new TableRow(children: new List<Widget>());
      for (int j = 0; j < 3; j++) {
        row.children.add(SubSudokuBoard(i, j));
      }
      subs.add(row);
    }
    return subs;
  }
}

class SubSudokuBoard extends StatelessWidget {
  final int row, col;

  SubSudokuBoard(this.row, this.col);

  @override
  Widget build(BuildContext context) {
    return Table(
      defaultVerticalAlignment: TableCellVerticalAlignment.middle,
      border: TableBorder(
        horizontalInside: BorderSide(
            width: 0,
            style: BorderStyle.solid
        ),
        verticalInside: BorderSide(
            width: 0,
            style: BorderStyle.solid
        ),
      ),
      children: getSudokuCells(),
    );
  }

  List<TableRow> getSudokuCells() {
    List<TableRow> cells = new List<TableRow>();
    for (int i = 0; i < 3; i++) {
      TableRow r = new TableRow(children: new List<Widget>());
      for (int j = 0; j < 3; j++) {
        r.children.add(SudokuCell(3 * row + i, 3 * col + j));
      }
      cells.add(r);
    }
    return cells;
  }
}

class SudokuCell extends StatelessWidget {
  final int row, col;

  SudokuCell(this.row, this.col);

  @override
  Widget build(BuildContext context) {
    return Consumer2<SudokuModel, ThemeModel>(
        builder: (context, sudoku, theme, child) {
          return Container(
              color: getColor(context, sudoku, theme),
              child: InkResponse(
                enableFeedback: true,
                onTap: () => onTap(sudoku),
                child: AspectRatio(
                    aspectRatio: 1,
                    child: getContents(sudoku)
                ),
              )
          );
        }
    );
  }

  Color getColor(BuildContext context, SudokuModel sudoku, ThemeModel theme) {
    int r = (row / 3).floor();
    int c = (col / 3).floor();
    if (sudoku.getValue(row, col) == sudoku.selectedNumber) // number matches selected digit
      return theme.selectedCellColor;
    else if (sudoku.selectedCellR != -1
        && sudoku.selectedCellC != -1
        && sudoku.getValue(sudoku.selectedCellR, sudoku.selectedCellC) == sudoku.getValue(row, col)
        && sudoku.getValue(row, col) != null) // number matches selected cell's digit
      return theme.selectedCellColor;
    else if (sudoku.selectedCellR == row && sudoku.selectedCellC == col) // this cell is selected
      return theme.selectedCellColor;
    else if (r == c) // is on the diagonal
      return theme.boardColor2;
    else if (r + c == 2) // is a corner
      return theme.boardColor2;
    return theme.boardColor1;
  }

  Widget getContents(SudokuModel model) {
    int value = model.getValue(row, col);
    if (value == null) {
      return Padding(
          padding: EdgeInsets.all(2),
          child: PencilMarks(row, col, model)
      );
    } else {
      return FittedBox(
          fit: BoxFit.contain,
          alignment: Alignment.center,
          child: getText(model, value)
      );
    }
  }

  Widget getText(SudokuModel model, int value) {
    if (model.given[row][col]) {
      return Text(value.toString(),
        style: TextStyle(fontWeight: FontWeight.bold),
      );
    } else {
      return Text(value.toString(),
        style: TextStyle(fontWeight: FontWeight.w300),
      );
    }
  }

  void onTap(SudokuModel model) {
    debugPrint('pressed cell $row, $col');
    if (model.getMode() == InsertMode.Eraser) { // eraser mode, remove value
      model.remove(row, col);
    } else if (model.selectedNumber != -1) { // number selected, set value
      if (model.getMode() == InsertMode.Pencil &&
          model.marks[row][col].contains(model.selectedNumber))
        model.removePencil(row, col, model.selectedNumber);
      else if (model.getMode() == InsertMode.Pencil &&
          !model.marks[row][col].contains(model.selectedNumber))
        model.addPencil(row, col, model.selectedNumber);
      else
        model.setValue(row, col, model.selectedNumber);
    } else { // no number selected, select cell
      model.setSelectedCell(row, col);
    }
  }
}

class PencilMarks extends StatelessWidget {
  final int row, col;
  final SudokuModel model;

  PencilMarks(this.row, this.col, this.model);

  @override
  Widget build(BuildContext context) {
    return Table(
      //border: TableBorder.all(),
      children: getPencilMarks(context),
    );
  }

  List<TableRow> getPencilMarks(BuildContext context) {
    List<TableRow> marks = new List<TableRow>();
    for (int i = 0; i < 3; i++) {
      TableRow r = new TableRow(children: new List<Widget>());
      for (int j = 0; j < 3; j++) {
        int value = 3 * i + j + 1;
        r.children.add(getPencilMark(context, value));
      }
      marks.add(r);
    }
    return marks;
  }

  Widget getPencilMark(BuildContext context, int value) {
    return FittedBox(
        fit: BoxFit.contain,
        child: SizedBox(
            height: 1,
            width: 1,
            child: FittedBox(
                fit: BoxFit.contain,
                alignment: Alignment.center,
                child: Text(getText(value)
                )
            )
        )
    );
  }

  String getText(int value) {
    if (model.marks[row][col].contains(value))
      return value.toString();
    else
      return '';
  }
}
