# \# Stuller Technical Assessment


# \## Projects

1\. Calculator Application

2\. Rhino House Plugin


# \## Setup

# 
# 1. Calculator

A simple calculator app built with C# and Avalonia UI. Tried to make it look and feel close to the standard Windows calculator.

## What's in here

- **Calculator.Core** – all the actual logic (no UI stuff in here)
- **Calculator.App** – the UI, built with Avalonia so it runs on Windows, Mac and Linux
- **Calculator.Tests** – unit tests for the core logic

## Running it

You'll need the .NET 8 SDK installed. If you don't have it:

```
winget install Microsoft.DotNet.SDK.8
```

Then install the Avalonia templates (only needed once):

```
dotnet new install Avalonia.Templates
```

Clone/download the repo and run:

```
dotnet run --project Calculator.App/Calculator.App.csproj
```

## Running the tests

```
dotnet test Calculator.Tests/Calculator.Tests.csproj
```

## Notes

- Uses MVVM pattern – the core engine has no dependency on Avalonia at all, makes it easier to test
- Left-to-right evaluation like the real Windows standard calculator (no operator precedence)
- Avalonia was chosen over WPF mainly because it's cross-platform

# 2. Rhino House Plugin

A simple Rhino 8 plugin built with C# and RhinoCommon that creates a procedural house model inside Rhino based on user input. Built as part of the Stuller technical assessment.

## What's in here

- **HouseBuilderPlugin** – Rhino plugin project containing the command and geometry logic
- **CreateHouse Command** – the main command used to generate the house
- **Geometry Helpers** – helper methods used to build the house body, roof, door, and chimney

## Running it

You'll need Rhino 8 and Visual Studio 2022 installed.

Build the project to generate the `HouseBuilderPlugin.dll`.

Open Rhino 8

Run 

```
LoadPlugin
```

Select:

```
  HouseBuilderPlugin.dll
```

Run:

```
  CreateHouse
```

prompts the user for:
    Base point
    House size

## Notes

- Built using RhinoCommon APIs for procedural geometry generation
- Demonstrates custom Rhino command development
- Uses user input (base point and size) to generate geometry dynamically
- Focused on clean and simple procedural modeling logic
