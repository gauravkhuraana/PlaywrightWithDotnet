using Allure.NUnit;
using Allure.NUnit.Attributes;
using Framework.Api.Http;
using Framework.Api.Testing;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Api;

/// <summary>
/// Smoke-level health checks against the practise API base URL.
/// These verify the framework's API stack end-to-end without depending on schema details.
/// </summary>
[AllureNUnit]
[AllureSuite("API")]
[AllureFeature("Health")]
[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
public sealed class HealthTests : ApiTestBase
{
    [Test]
    [AllureStory("API root is reachable")]
    public async Task Root_Endpoint_Returns_Successful_Response()
    {
        var response = await Api.SendAsync(new ApiRequest
        {
            Verb = HttpVerb.Get,
            Path = "/",
        });

        response.StatusCode.Should().BeLessThan(500, "the API root must not return a server error");
        response.Duration.Should().BeLessThan(TimeSpan.FromSeconds(15));
    }

    [Test]
    [AllureStory("Correlation id is propagated")]
    public async Task Correlation_Id_Is_Generated_When_Not_Supplied()
    {
        var response = await Api.SendAsync(new ApiRequest
        {
            Verb = HttpVerb.Get,
            Path = "/",
        });

        response.CorrelationId.Should().NotBeNullOrWhiteSpace();
    }
}
