using System.Net;
using FluentAssertions;

namespace IntegrationTests;

public class StatusCheckTests
{
    private readonly HttpClient _client = new();

    [SetUp]
    public void Setup()
    {
        Helper.RebuildDb();
    }

    [Test]
    public async Task GetStatus()
    {
        var response = await _client.GetAsync(Helper.BaseUrl + "/status");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}