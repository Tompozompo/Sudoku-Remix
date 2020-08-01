import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'ThemeModel.dart';

class BrightnessToggle extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeModel>(
        builder: (context, theme, child) {
          return IconButton(
            icon: Icon(Icons.lightbulb_outline),
            color: theme.iconColor,
            padding: EdgeInsets.all(5.0),
            onPressed: () =>
                Provider.of<ThemeModel>(context, listen: false)
                    .toggleBrightness(),
          );
        }
    );
  }
}
