# Estragonia

## 📖 About

Estragonia is a bridge allowing to use the powerful [Avalonia UI](https://github.com/AvaloniaUI/Avalonia/) framework in the no less powerful [Godot](https://github.com/godotengine/godot/) game engine!

## ⚡Getting started

1. Have Godot 4.0.x with .NET support installed.
2. Install the `JLeb.Estragonia` NuGet package inside your project.
3. Initialize the Avalonia application using `UseGodot().SetupWithoutStarting()`.
4. Create a Godot `Control` node, assign it a script inheriting from `JLeb.Estragonia.AvaloniaControl` and populate its `Control` property with any valid Avalonia view.

For a more detailed guide, see the [step by step instructions](docs/setup.md).
