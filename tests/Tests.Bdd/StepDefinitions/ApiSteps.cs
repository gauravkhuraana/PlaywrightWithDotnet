using Framework.Api.Http;
using Framework.Configuration;
using Framework.Configuration.Models;
using Framework.Core.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace Tests.Bdd.StepDefinitions;

[Binding]
public sealed class ApiSteps
{
    private readonly ScenarioContext _scenario;
    private ApiClient? _api;
    private ApiResponse? _response;

    public ApiSteps(ScenarioContext scenario)
    {
        _scenario = scenario;
    }

    [Given("the API client is configured for the practise environment")]
    public async Task GivenApiClientIsConfigured()
    {
        var sp = FrameworkServiceProvider.Root;
        var factory = sp.GetRequiredService<ApiClientFactory>();
        _api = await factory.CreateAsync();
        _scenario["api"] = _api;
    }

    [When("I send a GET request to {string}")]
    public async Task WhenISendGetRequest(string path)
    {
        if (_api is null)
        {
            throw new InvalidOperationException("Call the Given step first.");
        }
        _response = await _api.SendAsync(new ApiRequest { Verb = HttpVerb.Get, Path = path });
    }

    [Then("the response status code should be less than {int}")]
    public void ThenStatusCodeLessThan(int max)
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().BeLessThan(max);
    }

    [AfterScenario]
    public async Task After()
    {
        if (_api is not null)
        {
            await _api.DisposeAsync();
        }
    }
}
