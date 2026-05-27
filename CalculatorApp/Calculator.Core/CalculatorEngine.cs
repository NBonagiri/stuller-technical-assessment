using System;
using System.Globalization;

namespace Calculator.Core;

public class CalculatorEngine
{
    private const int MaxDigits = 16;

    private string _buffer = "0";
    private double _accumulator;
    private BinaryOperation? _pendingOp;
    private BinaryOperation? _repeatOp;
    private double _repeatRhs;

    // True after pressing an operator or = – next digit replaces the display.
    private bool _freshEntry = true;

    // Did the user actually type something since the last operator press?
    // Needed so "5 + =" repeats with 5, but "5 + CE =" uses 0 (not 5).
    private bool _enteredRhs;

    private bool _hasError;
    private string _errorMessage = string.Empty;

    public string Display => _hasError ? _errorMessage : _buffer;
    public string Expression { get; private set; } = string.Empty;
    public bool HasError => _hasError;

    public double? CurrentValue() => _hasError ? null : ParseBuffer();

    public void DigitPressed(int digit)
    {
        if (digit < 0 || digit > 9)
            throw new ArgumentOutOfRangeException(nameof(digit), "Must be 0–9.");
        if (_hasError) return;

        _enteredRhs = true;

        if (_freshEntry)
        {
            _buffer = digit == 0 ? "0" : digit.ToString(CultureInfo.InvariantCulture);
            _freshEntry = false;
            return;
        }

        if (_buffer is "0" or "-0")
        {
            if (digit != 0)
                _buffer = _buffer == "-0"
                    ? "-" + digit.ToString(CultureInfo.InvariantCulture)
                    : digit.ToString(CultureInfo.InvariantCulture);
            return;
        }

        int count = 0;
        foreach (char c in _buffer)
            if (char.IsDigit(c)) count++;
        if (count >= MaxDigits) return;

        _buffer += digit.ToString(CultureInfo.InvariantCulture);
    }

    public void DecimalPressed()
    {
        if (_hasError) return;
        _enteredRhs = true;

        if (_freshEntry)
        {
            _buffer = "0.";
            _freshEntry = false;
            return;
        }

        if (!_buffer.Contains('.'))
            _buffer += ".";
    }

    public void BackspacePressed()
    {
        if (_hasError || _freshEntry) return;

        if (_buffer.Length == 1 || (_buffer.Length == 2 && _buffer[0] == '-'))
        {
            _buffer = "0";
            return;
        }

        _buffer = _buffer[..^1];
        if (_buffer.EndsWith('.'))
            _buffer = _buffer[..^1];
    }

    public void BinaryOpPressed(BinaryOperation op)
    {
        if (_hasError) return;

        double current = ParseBuffer();

        if (_pendingOp.HasValue && !_freshEntry)
        {
            if (!TryEvaluate(_accumulator, _pendingOp.Value, current, out double interim))
            {
                SetError("Cannot divide by zero");
                return;
            }
            _accumulator = interim;
            _buffer = FormatNum(interim);
        }
        else
        {
            _accumulator = current;
        }

        _pendingOp = op;
        _repeatOp = null;
        _freshEntry = true;
        _enteredRhs = false;
        Expression = $"{FormatNum(_accumulator)} {Symbol(op)}";
    }

    public void EqualsPressed()
    {
        if (_hasError) return;

        if (_pendingOp.HasValue)
        {
            double rhs = _enteredRhs ? ParseBuffer() : _accumulator;
            string expr = $"{FormatNum(_accumulator)} {Symbol(_pendingOp.Value)} {FormatNum(rhs)} =";

            if (!TryEvaluate(_accumulator, _pendingOp.Value, rhs, out double result))
            {
                Expression = expr;
                SetError("Cannot divide by zero");
                return;
            }

            _repeatOp = _pendingOp;
            _repeatRhs = rhs;
            Expression = expr;
            _pendingOp = null;
            _accumulator = result;
            _buffer = FormatNum(result);
        }
        else if (_repeatOp.HasValue)
        {
            double lhs = ParseBuffer();
            string expr = $"{FormatNum(lhs)} {Symbol(_repeatOp.Value)} {FormatNum(_repeatRhs)} =";

            if (!TryEvaluate(lhs, _repeatOp.Value, _repeatRhs, out double result))
            {
                Expression = expr;
                SetError("Cannot divide by zero");
                return;
            }

            Expression = expr;
            _accumulator = result;
            _buffer = FormatNum(result);
        }

        _freshEntry = true;
        _enteredRhs = false;
    }

    public void UnaryOpPressed(UnaryOperation op)
    {
        if (_hasError) return;
        double current = ParseBuffer();

        switch (op)
        {
            case UnaryOperation.Negate:
                if (_buffer != "0" && _buffer != "0.")
                    _buffer = _buffer.StartsWith('-') ? _buffer[1..] : "-" + _buffer;
                return;

            case UnaryOperation.Percent:
                double pct = (_pendingOp is BinaryOperation.Add or BinaryOperation.Subtract)
                    ? _accumulator * current / 100.0
                    : current / 100.0;
                _buffer = FormatNum(pct);
                _freshEntry = true;
                _enteredRhs = true;
                return;

            case UnaryOperation.Reciprocal:
                if (current == 0) { SetError("Cannot divide by zero"); return; }
                Expression = $"1/({FormatNum(current)})";
                _buffer = FormatNum(1.0 / current);
                _freshEntry = true;
                _enteredRhs = true;
                return;

            case UnaryOperation.Square:
                Expression = $"sqr({FormatNum(current)})";
                _buffer = FormatNum(current * current);
                _freshEntry = true;
                _enteredRhs = true;
                return;

            case UnaryOperation.SquareRoot:
                if (current < 0) { SetError("Invalid input"); return; }
                Expression = $"√({FormatNum(current)})";
                _buffer = FormatNum(Math.Sqrt(current));
                _freshEntry = true;
                _enteredRhs = true;
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(op));
        }
    }

    public void ClearPressed()
    {
        _buffer = "0";
        _accumulator = 0;
        _pendingOp = null;
        _repeatOp = null;
        _repeatRhs = 0;
        _freshEntry = true;
        _enteredRhs = false;
        _hasError = false;
        _errorMessage = string.Empty;
        Expression = string.Empty;
    }

    public void ClearEntryPressed()
    {
        if (_hasError) { ClearPressed(); return; }
        _buffer = "0";
        _freshEntry = false;
        _enteredRhs = true;
    }

    public void LoadValue(double value)
    {
        _hasError = false;
        _errorMessage = string.Empty;
        _buffer = FormatNum(value);
        _freshEntry = true;
        _enteredRhs = true;
    }

    private double ParseBuffer() =>
        double.TryParse(_buffer, NumberStyles.Any, CultureInfo.InvariantCulture, out double v) ? v : 0;

    private void SetError(string msg)
    {
        _hasError = true;
        _errorMessage = msg;
    }

    private static bool TryEvaluate(double left, BinaryOperation op, double right, out double result)
    {
        result = 0;
        if (op == BinaryOperation.Divide && right == 0) return false;
        result = op switch
        {
            BinaryOperation.Add      => left + right,
            BinaryOperation.Subtract => left - right,
            BinaryOperation.Multiply => left * right,
            BinaryOperation.Divide   => left / right,
            _ => throw new ArgumentOutOfRangeException(nameof(op))
        };
        return true;
    }

    // G15 avoids trailing zeros (4.0 → "4") and floating-point noise (0.1+0.2 → "0.3")
    private static string FormatNum(double v) =>
        v.ToString("G15", CultureInfo.InvariantCulture);

    private static string Symbol(BinaryOperation op) => op switch
    {
        BinaryOperation.Add      => "+",
        BinaryOperation.Subtract => "-",
        BinaryOperation.Multiply => "×",
        BinaryOperation.Divide   => "÷",
        _ => "?"
    };
}
