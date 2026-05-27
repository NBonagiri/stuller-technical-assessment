using Avalonia.Controls;
using Avalonia.Input;
using Calculator.App.ViewModels;

namespace Calculator.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (DataContext is not MainViewModel vm) return;

        bool shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        bool ctrl  = e.KeyModifiers.HasFlag(KeyModifiers.Control);

        bool handled = true;

        switch (e.Key)
        {
            // Digits ─ main row (D8 needs special handling because Shift+8 = *)
            case Key.D0 or Key.NumPad0: vm.PressDigitCommand.Execute("0"); break;
            case Key.D1 or Key.NumPad1: vm.PressDigitCommand.Execute("1"); break;
            case Key.D2 or Key.NumPad2: vm.PressDigitCommand.Execute("2"); break;
            case Key.D3 or Key.NumPad3: vm.PressDigitCommand.Execute("3"); break;
            case Key.D4 or Key.NumPad4: vm.PressDigitCommand.Execute("4"); break;
            case Key.D6 or Key.NumPad6: vm.PressDigitCommand.Execute("6"); break;
            case Key.D7 or Key.NumPad7: vm.PressDigitCommand.Execute("7"); break;
            case Key.D9 or Key.NumPad9: vm.PressDigitCommand.Execute("9"); break;

            // D5: digit 5 or % (Shift+5 on US layout)
            case Key.D5:
                if (shift) vm.PressUnaryCommand.Execute("percent");
                else vm.PressDigitCommand.Execute("5");
                break;
            case Key.NumPad5: vm.PressDigitCommand.Execute("5"); break;

            // D8: digit 8 or * (Shift+8 on US layout)
            case Key.D8:
                if (shift) vm.PressOperatorCommand.Execute("*");
                else vm.PressDigitCommand.Execute("8");
                break;
            case Key.NumPad8: vm.PressDigitCommand.Execute("8"); break;

            // Decimal
            case Key.OemPeriod or Key.OemComma or Key.Decimal:
                vm.PressDecimalCommand.Execute(null); break;

            // Operators
            case Key.Add:
                vm.PressOperatorCommand.Execute("+"); break;
            case Key.OemPlus when shift:   // Shift+= → + on US layout
                vm.PressOperatorCommand.Execute("+"); break;
            case Key.Subtract or Key.OemMinus:
                vm.PressOperatorCommand.Execute("-"); break;
            case Key.Multiply:
                vm.PressOperatorCommand.Execute("*"); break;
            case Key.Divide:
                vm.PressOperatorCommand.Execute("/"); break;
            case Key.OemQuestion when !shift:   // / key (unshifted) on many layouts
                vm.PressOperatorCommand.Execute("/"); break;

            // Equals / Enter
            case Key.Enter or Key.Return:
                vm.PressEqualsCommand.Execute(null); break;
            case Key.OemPlus when !shift:  // plain = key (unshifted OemPlus)
                vm.PressEqualsCommand.Execute(null); break;

            // Clear
            case Key.Escape:
                vm.PressClearCommand.Execute(null); break;
            case Key.Delete:
                vm.PressClearEntryCommand.Execute(null); break;

            // Backspace
            case Key.Back:
                vm.PressBackspaceCommand.Execute(null); break;

            // Unary shortcuts
            case Key.F9:
                vm.PressUnaryCommand.Execute("negate"); break;
            case Key.R when !ctrl:
                vm.PressUnaryCommand.Execute("reciprocal"); break;
            case Key.Q when !ctrl:
                vm.PressUnaryCommand.Execute("square"); break;

            // Memory shortcuts (Ctrl combos)
            case Key.M when ctrl:
                vm.PressMemoryStoreCommand.Execute(null); break;
            case Key.R when ctrl:
                vm.PressMemoryRecallCommand.Execute(null); break;
            case Key.P when ctrl:
                vm.PressMemoryAddCommand.Execute(null); break;
            case Key.Q when ctrl:
                vm.PressMemorySubtractCommand.Execute(null); break;
            case Key.L when ctrl:
                vm.PressMemoryClearCommand.Execute(null); break;

            default:
                handled = false;
                break;
        }

        if (handled) e.Handled = true;
    }
}
