using Bogus;

namespace Framework.Data.Fakers;

/// <summary>
/// Central <see cref="Faker"/> registry. Use a stable seed in CI for reproducible data.
/// </summary>
public static class FakerRegistry
{
    /// <summary>Cached default <see cref="Faker"/> instance.</summary>
    public static readonly Faker Faker = new("en");

    /// <summary>Returns a new <see cref="Faker{T}"/> seeded by the given key (deterministic per key).</summary>
    public static Faker<T> For<T>(string seedKey)
        where T : class
    {
        var faker = new Faker<T>("en");
        faker.UseSeed(seedKey.GetHashCode(StringComparison.Ordinal));
        return faker;
    }
}
