import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'SudokuModel.dart';
import 'ThemeModel.dart';

class UtilRow extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer2<SudokuModel, ThemeModel>(
        builder: (context, sudoku, theme, child) {
          return Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            crossAxisAlignment: CrossAxisAlignment.end,
            children: <Widget>[
              ToggleButtons(
                children: <Widget>[
                  Icon(Icons.add),
                  Icon(Icons.border_color),
                  Icon(Icons.clear),
                ],
                isSelected: sudoku.mode,
                borderWidth: 1,
                color: theme.iconColor,
                borderColor: theme.borderColor,
                selectedBorderColor: theme.borderColor,
                fillColor: theme.selectedCellColor,
                selectedColor: Theme.of(context).textTheme.button.color,
                onPressed: (int index) {
                  sudoku.setMode(index);
                },
              ),
              Spacer(),
              IconButton(
                color: theme.iconColor,
                onPressed: () {
                  Provider.of<SudokuModel>(context, listen: false)
                      .undo();
                },
                icon: Icon(Icons.undo),
              ),
              IconButton(
                color: theme.iconColor,
                onPressed: () {
                  Provider.of<SudokuModel>(context, listen: false)
                      .redo();
                },
                icon: Icon(Icons.redo),
              ),
            ],
          );
        }
    );
  }
}
