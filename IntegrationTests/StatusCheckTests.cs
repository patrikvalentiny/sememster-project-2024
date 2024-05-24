using System.Net;
using FluentAssertions;

namespace IntegrationTests;

public class StatusCheckTests
{
    private readonly HttpClient _client = new();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetStatus()
    {
        var response = await _client.GetAsync("http://localhost:8080/api/v1/status");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}