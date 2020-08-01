import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import 'SudokuModel.dart';

class Time extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.center,
      mainAxisAlignment: MainAxisAlignment.center,
      children: <Widget>[
        Consumer<SudokuModel>(
          builder: (context, model, child) {
            return Text(getTimerString(model)
            );
          }
        ),
      ],
    );
  }

  String getTimerString(SudokuModel model) {
    final f = new NumberFormat("00");
    String seconds = f.format(model.timer.elapsed.inSeconds % 60);
    String minutes = f.format(model.timer.elapsed.inMinutes % 60);
    return "$minutes:$seconds";
  }
}
