using NUnit.Framework;
using Tests.Common;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

namespace Tests.UI;

[SetUpFixture]
public sealed class AssemblyBootstrap
{
    [OneTimeSetUp]
    public void Setup() => TestBootstrap.Initialize();
}
