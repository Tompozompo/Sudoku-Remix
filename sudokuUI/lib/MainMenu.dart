import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:sudoku/SudokuGenerator/DifficultyOption.dart';

import 'GamePage.dart';
import 'SudokuModel.dart';
import 'ThemeModel.dart';
import 'ColorPicker.dart';
import 'BrightnessToggle.dart';

class MainMenu extends StatefulWidget {

  @override
  State<StatefulWidget> createState() => MainMenuState();
}

class MainMenuState extends State<MainMenu> {
  final List<String> difficulties = ["Easy", "Medium", "Advanced", "Expert", "Master"];
  int difficultyIndex = 3;
  bool right = true;

  final List<String> ruleSet = ["Classic", "Random", "Chess", "Custom"];
  int ruleIndex = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
            elevation: 0.0,
            backgroundColor: Theme
                .of(context)
                .scaffoldBackgroundColor,
            actions: <Widget>[
              BrightnessToggle(),
              ColorPicker(),
            ]
        ),
        body: Consumer<ThemeModel>(
            builder: (context, theme, child) {
              return Column(
                mainAxisAlignment: MainAxisAlignment.center,
                mainAxisSize: MainAxisSize.max,
                children: <Widget>[
                  Row(
                      children: <Widget>[
                        Spacer(),
                        Expanded(
                          flex: 5,
                          child: FittedBox(
                              fit: BoxFit.contain,
                              child: Icon(Icons.sentiment_very_satisfied,
                                  color: theme.iconColor
                              )
                          ),
                        ),
                        Spacer()
                      ]
                  ),
                  Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: <Widget>[
                        Spacer(),
                        IconButton(
                          icon: Icon(Icons.arrow_back),
                          onPressed: () => backPressRule(),
                        ),
                        Expanded(
                          flex: 2,
                          child: GestureDetector(
                            onHorizontalDragEnd: (details) {
                              debugPrint(details.velocity.pixelsPerSecond.direction.toString());
                              if (details.primaryVelocity > 0) forwardPressRule();
                              if (details.primaryVelocity < 0) backPressRule();
                            },
                            child: Container(
                                alignment: Alignment.center,
                                child: AnimatedSwitcher(
                                    duration: const Duration(milliseconds: 200),
                                    transitionBuilder: (Widget child, Animation<double> animation) {
                                      final rAnimation = Tween<Offset>(begin: Offset(2.0, 0.0), end: Offset(0.0, 0.0))
                                          .animate(animation);
                                      final lAnimation = Tween<Offset>(begin: Offset(-2.0, 0.0), end: Offset(0.0, 0.0))
                                          .animate(animation);
                                      if (child.key == ValueKey(ruleIndex)) {
                                        return ClipRect(
                                          child: SlideTransition(
                                            position: right ? rAnimation : lAnimation,
                                            child: Padding(
                                              padding: const EdgeInsets.all(20.0),
                                              child: child,
                                            ),
                                          ),
                                        );
                                      } else {
                                        return ClipRect(
                                          child: SlideTransition(
                                            position: right ? lAnimation : rAnimation,
                                            child: Padding(
                                              padding: const EdgeInsets.all(20.0),
                                              child: child,
                                            ),
                                          ),
                                        );
                                      }
                                    },
                                    child: getRule()
                                )
                            ),
                          ),
                        ),
                        IconButton(
                          icon: Icon(Icons.arrow_forward),
                          onPressed: () => forwardPressRule(),
                        ),
                        Spacer(),
                      ]
                  ),
                  Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: <Widget>[
                        Spacer(),
                        IconButton(
                          icon: Icon(Icons.arrow_back),
                          onPressed: () => backPressDifficulty(),
                        ),
                        Expanded(
                            flex: 2,
                            child: GestureDetector(
                              onHorizontalDragEnd: (details) {
                                debugPrint(details.velocity.pixelsPerSecond.direction.toString());
                                if (details.primaryVelocity > 0) forwardPressDifficulty();
                                if (details.primaryVelocity < 0) backPressDifficulty();
                              },
                              child: Container(
                                  alignment: Alignment.center,
                                  child: AnimatedSwitcher(
                                      duration: const Duration(milliseconds: 200),
                                      transitionBuilder: (Widget child,
                                          Animation<double> animation) {
                                        final rAnimation =
                                        Tween<Offset>(begin: Offset(2.0, 0.0), end: Offset(0.0, 0.0))
                                            .animate(animation);
                                        final lAnimation =
                                        Tween<Offset>(begin: Offset(-2.0, 0.0), end: Offset(0.0, 0.0))
                                            .animate(animation);
                                        if (child.key == ValueKey(difficultyIndex)) {
                                          return ClipRect(
                                            child: SlideTransition(
                                              position: right ? rAnimation : lAnimation,
                                              child: Padding(
                                                padding: const EdgeInsets.all(20.0),
                                                child: child,
                                              ),
                                            ),
                                          );
                                        } else {
                                          return ClipRect(
                                            child: SlideTransition(
                                              position: right ? lAnimation : rAnimation,
                                              child: Padding(
                                                padding: const EdgeInsets.all(20.0),
                                                child: child,
                                              ),
                                            ),
                                          );
                                        }
                                      },
                                      child: getDifficulty()
                                  )
                              ),
                            )
                        ),
                        IconButton(
                          icon: Icon(Icons.arrow_forward),
                          onPressed: () => forwardPressDifficulty(),
                        ),
                        Spacer(),
                      ]
                  ),
                  Spacer(),
                  Row(
                      children: <Widget>[
                        Spacer(),
                        Expanded(
                          flex: 2,
                          child: RaisedButton(
                            color: theme.buttonColor,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(50.0),
                              side: BorderSide(color: theme.borderColor),
                            ),
                            child: Text('New Game'),
                            onPressed: () {
                              Provider.of<SudokuModel>(context, listen: false)
                                  .randomPuzzle(DifficultyOption.values[difficultyIndex]).then((value) {
                                Navigator.push(
                                    context,
                                    MaterialPageRoute(builder: (context) {
                                      return GamePage();
                                    })
                                );
                              }
                              );
                            },
                          ),
                        ),
                        Spacer(),
                      ]
                  ),
                  Row(
                      children: <Widget>[
                        Spacer(),
                        Expanded(
                          flex: 2,
                          child: RaisedButton(
                            color: theme.buttonColor,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(50.0),
                              side: BorderSide(color: theme.borderColor),
                            ),
                            child: Text('Resume Game'),
                            onPressed: () {
                              Navigator.push(
                                  context,
                                  MaterialPageRoute(builder: (context) {
                                    return GamePage();
                                  })
                              );
                            },
                          ),
                        ),
                        Spacer(),
                      ]
                  ),
                  Spacer(
                    flex: 1,
                  ),
                ],
              );
            }
        )
    );
  }

  Widget getDifficulty() {
    return Text(difficulties[difficultyIndex],
      key: ValueKey(difficultyIndex),
      style: Theme
          .of(context)
          .textTheme
          .headline5,
    );
  }

  Widget getRule() {
    return Text(ruleSet[ruleIndex],
      key: ValueKey(ruleIndex),
      style: Theme
          .of(context)
          .textTheme
          .headline5,
    );
  }

  void forwardPressRule() {
    setState(() {
      ruleIndex = ruleIndex < ruleSet.length - 1
          ? ruleIndex + 1
          : ruleIndex;
      right = true;
    });
  }

  void backPressRule() {
    setState(() {
      ruleIndex = ruleIndex > 0
          ? ruleIndex - 1
          : ruleIndex;
      right = false;
    });
  }


  void forwardPressDifficulty() {
    setState(() {
      difficultyIndex = difficultyIndex < difficulties.length - 1
          ? difficultyIndex + 1
          : difficultyIndex;
      right = true;
    });
  }

  void backPressDifficulty() {
    setState(() {
      difficultyIndex = difficultyIndex > 0
          ? difficultyIndex - 1
          : difficultyIndex;
      right = false;
    });
  }
}
