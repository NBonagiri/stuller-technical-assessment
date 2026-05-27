namespace Calculator.Core;

public class MemoryStore
{
    private double? _stored;

    public bool HasValue => _stored.HasValue;

    // Returns 0 if nothing has been stored (matches Windows Calc MR behaviour).
    public double Recall() => _stored ?? 0;

    public void Store(double value) => _stored = value;

    public void Add(double value) => _stored = (_stored ?? 0) + value;

    public void Subtract(double value) => _stored = (_stored ?? 0) - value;

    public void Clear() => _stored = null;
}
