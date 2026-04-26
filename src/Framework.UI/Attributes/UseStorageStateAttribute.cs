namespace Framework.UI.Attributes;

/// <summary>
/// Declares that a test fixture / test should reuse a previously-saved Playwright
/// <c>storageState</c> for the named role (e.g. <c>admin</c>, <c>customer</c>).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class UseStorageStateAttribute : Attribute
{
    public UseStorageStateAttribute(string role)
    {
        Role = role;
    }

    public string Role { get; }
}
