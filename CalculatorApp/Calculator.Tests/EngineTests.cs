using Calculator.Core;

namespace Calculator.Tests;

public class EngineTests
{
    // Helper: runs a string of button presses against a fresh engine.
    // Symbols: digits 0-9, . + - * / = C E(CE) <(backspace) N(+/-) S(√) Q(x²) R(1/x) %
    private static CalculatorEngine Run(string seq)
    {
        var e = new CalculatorEngine();
        foreach (char c in seq)
        {
            switch (c)
            {
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    e.DigitPressed(c - '0'); break;
                case '.': e.DecimalPressed(); break;
                case '+': e.BinaryOpPressed(BinaryOperation.Add); break;
                case '-': e.BinaryOpPressed(BinaryOperation.Subtract); break;
                case '*': e.BinaryOpPressed(BinaryOperation.Multiply); break;
                case '/': e.BinaryOpPressed(BinaryOperation.Divide); break;
                case '=': e.EqualsPressed(); break;
                case 'C': e.ClearPressed(); break;
                case 'E': e.ClearEntryPressed(); break;
                case '<': e.BackspacePressed(); break;
                case 'N': e.UnaryOpPressed(UnaryOperation.Negate); break;
                case 'S': e.UnaryOpPressed(UnaryOperation.SquareRoot); break;
                case 'Q': e.UnaryOpPressed(UnaryOperation.Square); break;
                case 'R': e.UnaryOpPressed(UnaryOperation.Reciprocal); break;
                case '%': e.UnaryOpPressed(UnaryOperation.Percent); break;
            }
        }
        return e;
    }

    // ── Initial state ──────────────────────────────────────────────────────

    [Fact]
    public void Display_StartsAtZero() =>
        Assert.Equal("0", new CalculatorEngine().Display);

    // ── Basic arithmetic ───────────────────────────────────────────────────

    [Theory]
    [InlineData("5+3=",  "8")]
    [InlineData("9-4=",  "5")]
    [InlineData("3*4=",  "12")]
    [InlineData("10/2=", "5")]
    public void BasicOperations_GiveCorrectResult(string input, string expected) =>
        Assert.Equal(expected, Run(input).Display);

    [Fact]
    public void SubtractLarger_GivesNegative() =>
        Assert.Equal("-2", Run("3-5=").Display);

    [Fact]
    public void DecimalArithmetic_NoFloatNoise() =>
        // 0.1 + 0.2 must show 0.3, not 0.30000000000000004
        Assert.Equal("0.3", Run("0.1+0.2=").Display);

    [Fact]
    public void ChainedOps_EvaluateLeftToRight() =>
        // 5 + 3 * 2 = (5+3)*2 = 16 (no precedence in Standard mode)
        Assert.Equal("16", Run("5+3*2=").Display);

    // ── Equals repeat behaviour ────────────────────────────────────────────

    [Fact]
    public void RepeatedEquals_ReappliesLastOp()
    {
        var e = new CalculatorEngine();
        e.DigitPressed(5);
        e.BinaryOpPressed(BinaryOperation.Add);
        e.DigitPressed(3);
        e.EqualsPressed(); // 8
        e.EqualsPressed(); // 11
        e.EqualsPressed(); // 14
        Assert.Equal("14", e.Display);
    }

    [Fact]
    public void EqualsWithNoRhs_UsesLhsAsRhs()
    {
        // 5 + = → 5 + 5 = 10
        var e = new CalculatorEngine();
        e.DigitPressed(5);
        e.BinaryOpPressed(BinaryOperation.Add);
        e.EqualsPressed();
        Assert.Equal("10", e.Display);
    }

    // ── Division by zero ───────────────────────────────────────────────────

    [Fact]
    public void DivideByZero_ShowsError()
    {
        var e = Run("5/0=");
        Assert.True(e.HasError);
        Assert.Contains("zero", e.Display, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AfterError_ClearResetsState()
    {
        var e = Run("5/0=");
        e.ClearPressed();
        Assert.False(e.HasError);
        Assert.Equal("0", e.Display);
    }

    // ── Unary operations ───────────────────────────────────────────────────

    [Fact]
    public void Negate_TogglesSign() =>
        Assert.Equal("-5", Run("5N").Display);

    [Fact]
    public void Negate_Twice_RestoresSign() =>
        Assert.Equal("5", Run("5NN").Display);

    [Fact]
    public void SquareRoot_PerfectSquare() =>
        Assert.Equal("3", Run("9S").Display);

    [Fact]
    public void SquareRoot_OfNegative_SetsError() =>
        Assert.True(Run("4NS").HasError);

    [Fact]
    public void Square_BasicValue() =>
        Assert.Equal("9", Run("3Q").Display);

    [Fact]
    public void Reciprocal_BasicValue() =>
        Assert.Equal("0.25", Run("4R").Display);

    [Fact]
    public void Reciprocal_OfZero_SetsError() =>
        Assert.True(Run("0R").HasError);

    [Fact]
    public void Percent_InAddContext_IsPercentOfAccumulator()
    {
        // 200 + 10% → display shows 20 (10% of 200), then = gives 220
        var e = new CalculatorEngine();
        foreach (char c in "200") e.DigitPressed(c - '0');
        e.BinaryOpPressed(BinaryOperation.Add);
        e.DigitPressed(1); e.DigitPressed(0);
        e.UnaryOpPressed(UnaryOperation.Percent);
        Assert.Equal("20", e.Display);
        e.EqualsPressed();
        Assert.Equal("220", e.Display);
    }

    // ── Clear (C) and Clear Entry (CE) ─────────────────────────────────────

    [Fact]
    public void Clear_ResetsEverything()
    {
        var e = Run("5+3");
        e.ClearPressed();
        Assert.Equal("0", e.Display);
        Assert.Equal(string.Empty, e.Expression);
    }

    [Fact]
    public void ClearEntry_KeepsPendingOp()
    {
        // 5 + 3 CE → clears 3, = uses 0 → result 5
        var e = new CalculatorEngine();
        e.DigitPressed(5);
        e.BinaryOpPressed(BinaryOperation.Add);
        e.DigitPressed(3);
        e.ClearEntryPressed();
        Assert.Equal("0", e.Display);
        e.EqualsPressed();
        Assert.Equal("5", e.Display);
    }

    // ── Backspace ──────────────────────────────────────────────────────────

    [Fact]
    public void Backspace_RemovesLastDigit() =>
        Assert.Equal("12", Run("123<").Display);

    [Fact]
    public void Backspace_SingleDigit_BecomesZero() =>
        Assert.Equal("0", Run("7<").Display);

    // ── Digit entry ────────────────────────────────────────────────────────

    [Fact]
    public void NoLeadingZeros() =>
        Assert.Equal("0", Run("00").Display);

    [Fact]
    public void DigitAfterResult_StartsNew() =>
        Assert.Equal("7", Run("5+3=7").Display);

    // ── Decimal ────────────────────────────────────────────────────────────

    [Fact]
    public void Decimal_WithNoLeadingDigit_Gives0Dot()
    {
        var e = new CalculatorEngine();
        e.DecimalPressed();
        Assert.Equal("0.", e.Display);
    }

    [Fact]
    public void SecondDecimal_Ignored() =>
        Assert.Equal("3.14", Run("3..14").Display);
}
