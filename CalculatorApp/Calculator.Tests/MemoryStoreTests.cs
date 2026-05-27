using Calculator.Core;

namespace Calculator.Tests;

public class MemoryStoreTests
{
    [Fact]
    public void HasValue_FalseByDefault() =>
        Assert.False(new MemoryStore().HasValue);

    [Fact]
    public void Recall_WithoutStore_ReturnsZero() =>
        Assert.Equal(0.0, new MemoryStore().Recall());

    [Fact]
    public void Store_ThenRecall_GivesStoredValue()
    {
        var m = new MemoryStore();
        m.Store(42.5);
        Assert.Equal(42.5, m.Recall());
    }

    [Fact]
    public void Add_AccumulatesOntoExisting()
    {
        var m = new MemoryStore();
        m.Store(10.0);
        m.Add(5.0);
        Assert.Equal(15.0, m.Recall());
    }

    [Fact]
    public void Subtract_DecreasesExisting()
    {
        var m = new MemoryStore();
        m.Store(10.0);
        m.Subtract(4.0);
        Assert.Equal(6.0, m.Recall());
    }

    [Fact]
    public void Clear_RemovesStoredValue()
    {
        var m = new MemoryStore();
        m.Store(99.0);
        m.Clear();
        Assert.False(m.HasValue);
    }

    [Fact]
    public void Add_ToEmptySlot_StoresValue()
    {
        var m = new MemoryStore();
        m.Add(7.0);
        Assert.Equal(7.0, m.Recall());
    }
}
