# Input Tester

A lightweight Windows Forms utility for checking keyboard and mouse input.

![title](./TEST.JPG)

## Features

### Keyboard
- Detects key-down and key-up events while the application window is active.
- Displays key names and virtual-key codes.
- Counts key presses, including key auto-repeat.
- Shows the current key state (`DOWN` / `UP`).

### Mouse
- Displays pointer screen coordinates.
- Counts mouse movement events.
- Tests left, right, middle, fourth (`XButton1`), and fifth (`XButton2`) mouse buttons.
- Counts mouse-wheel movement in both directions.
- Displays recent input events.

### Other
- Reset counters to clear all statistics.
- AAYDEV logo embedded directly into the executable.
- No external image files required to run the application.

## Requirements

- Windows XP or later.
- .NET Framework 2.0 runtime.
- x86 architecture.

## Building

The project is designed for Visual Studio 2019.

1. Open `InputTester.sln`.
2. Select `Release` configuration and `x86` platform.
3. Select **Build → Rebuild Solution**.
4. The executable will be created in:

   `InputTester\bin\Release\InputTester.exe`

The logo is included in the project as an embedded resource. The `logo.png` file is required for building but does not need to accompany the executable.

## How to Use

1. Launch `InputTester.exe`.
2. Click the application window and press keys to test the keyboard.
3. Move the mouse over the application window.
4. Use the mouse test area to check buttons and scrolling.
5. Click **Reset counters** to clear the results.

## Limitations

- This is a window-based input tester, not a global input hook.
- Keyboard input is captured while the application window is active.
- Mouse input is captured while the pointer is over the application window.
- Secure system shortcuts such as `Ctrl+Alt+Del` cannot be reliably captured by a regular Windows Forms application.

## License

No license is currently specified. Add a license file before distributing the project as open-source software.
