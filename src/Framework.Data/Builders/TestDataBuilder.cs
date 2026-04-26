namespace Framework.Data.Builders;

/// <summary>
/// Generic fluent test-data builder. Concrete builders inherit and expose
/// strongly-typed With* methods producing immutable results via <see cref="Build"/>.
/// </summary>
public abstract class TestDataBuilder<TBuilder, TResult>
    where TBuilder : TestDataBuilder<TBuilder, TResult>
{
    protected TBuilder Self => (TBuilder)this;

    public abstract TResult Build();
}
