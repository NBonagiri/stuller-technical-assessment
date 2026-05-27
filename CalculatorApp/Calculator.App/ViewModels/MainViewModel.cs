using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Calculator.Core;

namespace Calculator.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly CalculatorEngine _engine = new();
    private readonly MemoryStore _memory = new();

    [ObservableProperty]
    private string _displayText = "0";

    [ObservableProperty]
    private string _expressionText = string.Empty;

    // Controls whether MR/M+/M-/MC are enabled.
    [ObservableProperty]
    private bool _memoryHasValue;

    // -----------------------------------------------------------------------
    // Digit / decimal / backspace
    // -----------------------------------------------------------------------

    [RelayCommand]
    private void PressDigit(string digit)
    {
        _engine.DigitPressed(int.Parse(digit));
        Refresh();
    }

    [RelayCommand]
    private void PressDecimal()
    {
        _engine.DecimalPressed();
        Refresh();
    }

    [RelayCommand]
    private void PressBackspace()
    {
        _engine.BackspacePressed();
        Refresh();
    }

    // -----------------------------------------------------------------------
    // Binary operators
    // -----------------------------------------------------------------------

    [RelayCommand]
    private void PressOperator(string op)
    {
        _engine.BinaryOpPressed(op switch
        {
            "+" => BinaryOperation.Add,
            "-" => BinaryOperation.Subtract,
            "*" => BinaryOperation.Multiply,
            "/" => BinaryOperation.Divide,
            _   => throw new ArgumentException($"Unrecognised operator '{op}'.", nameof(op))
        });
        Refresh();
    }

    [RelayCommand]
    private void PressEquals()
    {
        _engine.EqualsPressed();
        Refresh();
    }

    // -----------------------------------------------------------------------
    // Unary / special
    // -----------------------------------------------------------------------

    [RelayCommand]
    private void PressUnary(string op)
    {
        _engine.UnaryOpPressed(op switch
        {
            "negate"     => UnaryOperation.Negate,
            "percent"    => UnaryOperation.Percent,
            "reciprocal" => UnaryOperation.Reciprocal,
            "square"     => UnaryOperation.Square,
            "sqrt"       => UnaryOperation.SquareRoot,
            _ => throw new ArgumentException($"Unrecognised unary op '{op}'.", nameof(op))
        });
        Refresh();
    }

    [RelayCommand]
    private void PressClear()
    {
        _engine.ClearPressed();
        Refresh();
    }

    [RelayCommand]
    private void PressClearEntry()
    {
        _engine.ClearEntryPressed();
        Refresh();
    }

    // -----------------------------------------------------------------------
    // Memory
    // -----------------------------------------------------------------------

    [RelayCommand]
    private void PressMemoryStore()
    {
        double? v = _engine.CurrentValue();
        if (v is null) return;
        _memory.Store(v.Value);
        MemoryHasValue = _memory.HasValue;
    }

    [RelayCommand]
    private void PressMemoryRecall()
    {
        if (!_memory.HasValue) return;
        _engine.LoadValue(_memory.Recall());
        Refresh();
    }

    [RelayCommand]
    private void PressMemoryAdd()
    {
        double? v = _engine.CurrentValue();
        if (v is null) return;
        _memory.Add(v.Value);
        MemoryHasValue = _memory.HasValue;
    }

    [RelayCommand]
    private void PressMemorySubtract()
    {
        double? v = _engine.CurrentValue();
        if (v is null) return;
        _memory.Subtract(v.Value);
        MemoryHasValue = _memory.HasValue;
    }

    [RelayCommand]
    private void PressMemoryClear()
    {
        _memory.Clear();
        MemoryHasValue = false;
    }

    // -----------------------------------------------------------------------

    private void Refresh()
    {
        DisplayText   = _engine.Display;
        ExpressionText = _engine.Expression;
    }
}
