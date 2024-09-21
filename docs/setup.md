# Step by step setup

## Finished project

After following the steps below, you should have a project structure similar to the [HelloWorld sample](../samples/HelloWorld/).

(The HelloWorld sample has some extra UI elements in the XAML file that aren't included in the steps below, as they aren't relevant to setup Estragonia.)

## Godot setup

1. [Download Godot Engine](https://godotengine.org/download/) version 4.3.0 or later, with .NET support.
2. Create a new Godot project or open an existing one. It must use either the Forward+ or Mobile renderer.
3. Add a new Godot `Control` node to your scene. In this example, name it `UserInterface`.
4. In the inspector for the newly created control, under _Focus_, ensure that `Mode` is set to `All`. If you don't, Godot won't pass keyboard input to it.
5. Attach a new C# script to the previously created control. It should be automatically named `UserInterface.cs`.

📖 Refer to the official [Godot documentation](https://docs.godotengine.org/en/stable/) if needed.

## Avalonia setup

1. Open the .NET solution created by Godot in your favorite editor.
2. Install the [JLeb.Estragonia](https://www.nuget.org/packages/JLeb.Estragonia/) NuGet package inside your project.
3. Install an Avalonia NuGet theme package: we're going to use the [official fluent theme](https://www.nuget.org/packages/Avalonia.Themes.Fluent/). Without a theme, Avalonia won't display anything.
4. Change the previously created `UserInterface` class to inherit from `JLeb.Estragonia.AvaloniaControl` instead of `Godot.Control`.  
5. Either remove the auto-generated `_Ready` and `_Process` overrides or ensure the base method is called (otherwise the Avalonia control won't be rendered).
6. As with a normal Avalonia project, Estragonia needs an `Avalonia.Application`-derived class.   
   Create the `App` type in XAML (or C#), and ensure it has a `FluentTheme` style.
7. Initialize Avalonia using `AppBuilder.Configure<App>().UseGodot().SetupWithoutStarting()`.  
   While this can be called inside the `UserInterface._Ready` method, it will only work for a single control as the application must be initialized only once. If you plan to have several `AvaloniaControl` instances, we recommend to use a Godot [autoload script](https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html) or a C# singleton, which is what is done in the sample in the `AvaloniaLoader` class.
8. Create a new Avalonia view: it's recommended to inherit from `UserControl`. In this example, name it `HelloWorldView.axaml`.
9. Populate your view with Avalonia controls as you would do in a standard Avalonia application.  
   In this example, we're using a simple `<TextBlock Text="Welcome to Avalonia in Godot!" />`
10. Assign the view to the `UserInterface.Control` property in the `_Ready` method: `Control = new HelloWorldView()`.  
    Don't forget to call the `base._Ready()` after.

📖 Refer to the official [Avalonia documentation](https://docs.avaloniaui.net/) if needed.

## Designer support

Several Avalonia designers exist, see [this Avalonia documentation page](https://docs.avaloniaui.net/docs/next/get-started/set-up-an-editor).  
To get your Godot .NET project to be compatible with these designers, it needs a few things:
1. Add a new class with a `void Main()` and a `AppBuilder BuildAvaloniaApp()` methods. `Main()` won't be called by the designer: it's only used to find the `BuildAvaloniaApp()` method next to it.
2. In `BuildAvaloniaApp()`, return `AppBuilder.Configure<App>().UseSkia()`. Do NOT call `UseGodot` as the designer doesn't run inside Godot and doesn't have access to its functions. See [Designer.cs](../samples/HelloWorld/Designer.cs) for the final file.  
3. Set an executable output type for the project, by adding the `<OutputType>Exe</OutputType>` property to the .csproj file. This is needed to get `Main()` to be recognized as an entry point.
4. Ensure your XAML files don't use any Godot class directly.

## Recommendations

- While the scripts created by Godot don't use any namespace by default, we recommend using one anyway as it's pretty standard in .NET: it helps organizing types.
- Since Avalonia has nullable annotations, consider enabling [nullable reference types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references) using `<Nullable>enable</Nullable>` in your csproj file to get nullable warnings.  
