import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'SudokuModel.dart';
import 'MainMenuPage.dart';
import 'ThemeModel.dart';

void main() {
  runApp(
    ChangeNotifierProvider<SudokuModel>(
        create: (context) => SudokuModel(),
        child: ChangeNotifierProvider<ThemeModel>(
          create: (context) => ThemeModel(),
          child: MyApp(),
        )
    ),
  );
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeModel>(
        builder: (context, model, child) {
          return MaterialApp(
            title: 'Sudoku',
            theme: ThemeData(
              brightness: model.brightness,
              primarySwatch: model.primarySwatch,
              visualDensity: VisualDensity.adaptivePlatformDensity,
            ),
            home: MainMenuPage(),
          );
        }
    );
  }
}
