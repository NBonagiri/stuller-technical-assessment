# \# Stuller Technical Assessment

# 

# \## Technologies Used

# \- C#

# \- .NET MAUI

# \- CommunityToolkit.Mvvm

# \- RhinoCommon

# 

# \## Projects

# 1\. Calculator Application

# 2\. Rhino House Plugin

# 

# \## Features

# \- MVVM architecture

# \- Cross-platform UI using MAUI

# \- Rhino plugin geometry generation

# \- User input for house size and location

# 

# \## Rhino Plugin Features

# The Rhino plugin creates:

# \- House body

# \- Door

# \- Chimney

# \- Roof mesh

# 

# The command:

# CreateHouse

# 

# prompts the user for:

# \- Base point

# \- House size

# 

# \## Setup

# 
# Calculator

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

# \### Rhino Plugin

# 1\. Open Rhino 8

# 2\. Run LoadPlugin

# 3\. Select:

# &#x20;  HouseBuilderPlugin.dll

# 4\. Run:

# &#x20;  CreateHouse

