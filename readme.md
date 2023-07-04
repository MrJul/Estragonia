# Estragonia: Avalonia in Godot

![Avalonia + Godot](https://github.com/MrJul/Estragonia/blob/main/docs/av_plus_gd.png)

Estragonia is a bridge allowing the use of the powerful [Avalonia UI](https://github.com/AvaloniaUI/Avalonia/) framework in the no less powerful [Godot](https://github.com/godotengine/godot/) game engine!  

It's GPU accelerated using Vulkan, which is the main renderer used in Godot 4.

## Quick Start

1. Have Godot 4.1.0 RC2 with .NET support installed.
2. Install the `JLeb.Estragonia` NuGet package inside your project.
3. Initialize the Avalonia application using `UseGodot().SetupWithoutStarting()`.
4. Create a Godot `Control` node, assign it a script inheriting from `JLeb.Estragonia.AvaloniaControl` and populate its `Control` property with any valid Avalonia view.

For a more detailed guide, see the [step by step instructions](https: docs/setup.md).

## Resources

For various things to know regarding compatibility, rendering and input handling, see [this document](https://github.com/MrJul/Estragonia/docs/toknow.md).

Samples:
 - [HelloWorld](https://github.com/MrJul/Estragonia/samples/HelloWorld): a basic Avalonia-into-Godot setup.
 - [GameMenu](https://github.com/MrJul/Estragonia/samples/GameMenu): a functional game menu UI using the MVVM pattern, with controller support, UI animations and scaling.

## License

The whole Estragonia project source code is under the [MIT License](license.txt).  
Some specific licenses may apply to some assets used in the samples. See each sample for more information.

## Screenshot

![GameMenu](https://github.com/MrJul/Estragonia/assets/1623034/4eeb5f36-1964-479c-b8d7-fedc12fc10fd)

From the [GameMenu sample](https://github.com/MrJul/Estragonia/samples/GameMenu)
