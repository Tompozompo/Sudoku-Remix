import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'SudokuModel.dart';
import 'ThemeModel.dart';

class KeyPad2 extends StatelessWidget {
  final int numRows = 2;
  final int numColumns = 5;

  @override
  Widget build(BuildContext context) {
    return Table(
      defaultVerticalAlignment: TableCellVerticalAlignment.middle,
      children: getKeyPadRows(),
    );
  }

  List<TableRow> getKeyPadRows() {
    List<TableRow> subs = new List<TableRow>();
    for (int i = 0; i < numRows; i++) {
      TableRow row = new TableRow(children: new List<Widget>());
      for (int j = 0; j < numColumns; j++) {
        row.children.add(KeyPadCell(this.numColumns * i + j + 1));
      }
      subs.add(row);
    }
    return subs;
  }
}

class KeyPadCell extends StatelessWidget {
  final int number;

  KeyPadCell(this.number);

  @override
  Widget build(BuildContext context) {
    return Consumer2<SudokuModel, ThemeModel>(
        builder: (context, sudoku, theme, child) {
          return InkResponse(
            enableFeedback: true,
            onTap: () => onTap(sudoku),
            child: Center(
              child: AspectRatio(
                aspectRatio: 1,
                child: Padding(
                    padding: EdgeInsets.all(0),
                    child: Container(
                      decoration: BoxDecoration(
                          border: Border.all(color: theme.borderColor),
                          borderRadius: BorderRadius.all(Radius.circular(100.0)),
                          color: getColor(context, sudoku, theme)
                      ),
                      child: FittedBox(
                          fit: BoxFit.contain,
                          child: Text(number.toString())
                      ),
                    )
                ),
              ),
            ),
          );
        }
    );
  }

  Color getColor(BuildContext context, SudokuModel sudoku, ThemeModel theme) {
    int s = sudoku.selectedNumber;
    if(s == number)
      return theme.selectedCellColor;
    else
      return Theme.of(context).scaffoldBackgroundColor;
  }

  void onTap(SudokuModel model) {
    debugPrint('pressed button $number');
    if(model.selectedCellR != -1 && model.selectedCellC != -1) { // cell selected, insert value
      if(model.getMode() == InsertMode.Pencil && model.marks[model.selectedCellR][model.selectedCellC].contains(number))
        model.removePencil(model.selectedCellR, model.selectedCellC, number);
      else if(model.getMode() == InsertMode.Pencil && !model.marks[model.selectedCellR][model.selectedCellC].contains(number))
        model.addPencil(model.selectedCellR, model.selectedCellC, number);
      else
        model.setValue(model.selectedCellR, model.selectedCellC, number);
    } else {
      model.setSelectedNumber(number);
    }
  }
}
