import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'ThemeModel.dart';

class ColorPicker extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeModel>(
      builder: (context, theme, child) {
        return PopupMenuButton<Color>(
          onSelected: (Color value) => theme.setPrimarySwatch(value),
          icon: Icon(Icons.color_lens,
            color: theme.iconColor
          ),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.all(Radius.circular(5)),
          ),
          itemBuilder: (BuildContext context) {
            return theme.colors.map((Color choice) {
              return PopupMenuItem<Color>(
                value: choice,
                child: Container(
                  color: choice,
                  alignment: Alignment.center,
                  child: SizedBox(
                      height: 20,
                      width: 20
                  ),
                ),
              );
            }).toList();
          },
        );
      }
    );
  }
}
