import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:numberpicker/numberpicker.dart';
import 'package:sudoku/SudokuGenerator/DifficultyOption.dart';

import 'SudokuModel.dart';
import 'SudokuBoard.dart';
import 'KeyPad.dart';
import 'ThemeModel.dart';
import 'Time.dart';
import 'UtilRow.dart';
import 'ColorPicker.dart';
import 'BrightnessToggle.dart';


class GamePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeModel>(
        builder: (context, theme, child) {
          return Scaffold(
            appBar: AppBar(
              backgroundColor: Theme
                  .of(context)
                  .scaffoldBackgroundColor,
              iconTheme: IconThemeData(
                color: theme.iconColor, //change your color here
              ),
              elevation: 0.0,
              actions: <Widget>[
                BrightnessToggle(),
                ColorPicker(),
                PopupMenuButton<String>(
                  onSelected: (String value) => handleClickPopup(value, context),
                  icon: Icon(Icons.more_vert,
                      color: theme.iconColor
                  ),
                  itemBuilder: (BuildContext context) {
                    return {'Restart', 'Auto-fill Marks', 'Validate', 'Show Solution', 'Settings', 'Rules', 'About'}.map((String choice) {
                      return PopupMenuItem<String>(
                        value: choice,
                        child: Text(choice),
                      );
                    }).toList();
                  },
                )
              ],
            ),
            body: Column(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.center,
              children: <Widget>[
                Padding(
                  padding: EdgeInsets.all(5.0),
                  child: Time(),
                ),
                Padding(
                  padding: EdgeInsets.all(5.0),
                  child: SudokuBoard(),
                ),
                Padding(
                  padding: EdgeInsets.all(5.0),
                  child: KeyPad(),
                ),
                Padding(
                  padding: EdgeInsets.all(5.0),
                  child: UtilRow(),
                ),
              ],
            ),
          );
        }
    );
  }

  void handleClickPopup(String value, BuildContext context) {
    int markValue = 0;
    switch (value) {
      case 'Restart':
        showDialog(
            context: context,
            builder: (BuildContext context) {
              return AlertDialog(
                  title: Text("Restart?"),
                  content: Text("Are you sure?"),
                  actions: <Widget>[
                    FlatButton(
                        onPressed: () {
                          Provider.of<SudokuModel>(context, listen: false)
                              .restart();
                          Navigator.of(context).pop();
                        },
                        child: Text("Yes")
                    ),
                    FlatButton(
                        onPressed: () {
                          Navigator.of(context).pop();
                        },
                        child: Text("No")
                    )
                  ]
              );
            }
        );
        break;
      case 'Validate':
        showDialog(
            context: context,
            builder: (BuildContext context) {
              return AlertDialog(
                  title: Text("Validate?"),
                  content: Text("Are you sure?"),
                  actions: <Widget>[
                    Consumer<SudokuModel>(
                        builder: (context, model, child) {
                          return FlatButton(
                              onPressed: () {
                                Navigator.of(context).pop();
                                if (model.validate()) {
                                  showDialog(
                                      context: context,
                                      builder: (BuildContext context) {
                                        return AlertDialog(
                                            title: Text("Looking Good!")
                                        );
                                      }
                                  );
                                } else {
                                  showDialog(
                                      context: context,
                                      builder: (BuildContext context) {
                                        return AlertDialog(
                                            title: Text("Mistakes were found!")
                                        );
                                      }
                                  );
                                }
                              },
                              child: Text("Yes")
                          );
                        }
                    ),
                    FlatButton(
                        onPressed: () {
                          Navigator.of(context).pop();
                        },
                        child: Text("No")
                    )
                  ]
              );
            }
        );
        break;
      case 'Show Solution':
        showDialog(
            context: context,
            builder: (BuildContext context) {
              return AlertDialog(
                  title: Text("Show solution?"),
                  content: Text("Are you sure?"),
                  actions: <Widget>[
                    FlatButton(
                        onPressed: () {
                          Provider.of<SudokuModel>(context, listen: false)
                              .showSolution();
                          Navigator.of(context).pop();
                        },
                        child: Text("Yes")
                    ),
                    FlatButton(
                        onPressed: () {
                          Navigator.of(context).pop();
                        },
                        child: Text("No")
                    )
                  ]
              );
            }
        );
        break;
      case 'Auto-fill Marks':
        showDialog<int>(
          context: context,
          builder: (BuildContext context) {
            return new NumberPickerDialog.integer(
              minValue: 0,
              maxValue: 4,
              initialIntegerValue: markValue,
            );
          },
        ).then((num value) {
          if (value != null) {
            markValue = value;
            Provider.of<SudokuModel>(context, listen: false).showOptions(DifficultyOption.values[markValue]);
          }
        });
      /*
        showDialog(
            context: context,
            builder: (BuildContext context) {
              return AlertDialog(
                  title: Text("Show pencil markings?"),
                  content: Text("Select a level:"),
                  actions: <Widget>[
                    new NumberPicker.integer(
                        initialValue: markValue,
                        minValue: 0,
                        maxValue: 5,
                        onChanged: (value) => markValue = value,
                        highlightSelectedValue: false,
                    ),
                    FlatButton(
                      onPressed: () {
                        Provider.of<SudokuModel>(context, listen: false).showOptions(markValue);
                      },
                      child: Text("Do it"),
                    ),
                    FlatButton(
                        onPressed: () {
                          Navigator.of(context).pop();
                        },
                        child: Text("No")
                    )
                  ]
              );
            }
        );
         */
        break;
      case 'About':
        showDialog(
            context: context,
            builder: (BuildContext context) {
              return AboutDialog(
              );
            }
        );
        break;
    }
  }
}