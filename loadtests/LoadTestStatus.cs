using FluentAssertions;
using NBomber.Contracts;
using NBomber.CSharp;

namespace LoadTests;

public class LoadTestStatus
{
    [SetUp]
    public void Setup()
    {
    }

    //smoke test
    [Test]
    [Order(1)]
    public void Smoke()
    {
        var scenario = Scenario.Create("smoke_test_status", async context =>
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(Helper.BaseUrl + "/status");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithLoadSimulations(
                Simulation.RampingInject(
                    10,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10)
                ),
                Simulation.Inject(
                    10,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(30)
                ),
                Simulation.RampingInject(
                    0,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10)
                )
            );


        var stats = NBomberRunner.RegisterScenarios(scenario).Run();
        var scenarioStats = stats.ScenarioStats.Get("smoke_test_status");
        scenarioStats.Fail.Request.Percent.Should().BeLessThan(5);
        // scenarioStats.Ok.Latency.Percent95.Should().BeLessThan(400);
    }

    [Test]
    [Order(2)]
    public void Load()
    {
        var scenario = Scenario.Create("load_test_status", async context =>
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(Helper.BaseUrl + "/status");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithLoadSimulations(
                Simulation.RampingInject(
                    50,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(30)
                ),
                Simulation.Inject(
                    50,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(30)),
                Simulation.RampingInject(
                    0,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(30)
                )
            );
        
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        var scenarioStats = stats.ScenarioStats.Get("load_test_status");
        scenarioStats.Fail.Request.Percent.Should().BeLessThan(5);
    }
    
    [Test]
    [Order(3)]
    public void Soak()
    {
        var scenario = Scenario.Create("soak_test_status", async context =>
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(Helper.BaseUrl + "/status");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithLoadSimulations(
                Simulation.RampingInject(
                    10,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10)
                ),
                Simulation.Inject(
                    10,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromMinutes(10)
                ),
                Simulation.RampingInject(
                    0,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10)
                )
            );
    
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        var scenarioStats = stats.ScenarioStats.Get("soak_test_status");
        scenarioStats.Fail.Request.Percent.Should().BeLessThan(5);
    }
    
    // These tests are omitted because of the lack of proper computation resources
    
    // [Test]
    // public void Stress()
    // {
    //     var scenario = Scenario.Create("stress_test_status", async context =>
    //         {
    //            using var client = new HttpClient();
    //            var response = await client.GetAsync(Helper.BaseUrl + "/status");
    //            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
    //         })
    //         .WithLoadSimulations(
    //             Simulation.RampingInject(
    //                 200,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromSeconds(30)
    //             ),
    //             Simulation.Inject(
    //                 200,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromMinutes(1)
    //             ),
    //             Simulation.RampingInject(
    //                 0,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromSeconds(30)
    //             )
    //         );
    //
    //     var stats = NBomberRunner
    //         .RegisterScenarios(scenario)
    //         .Run();
    //     var scenarioStats = stats.ScenarioStats.Get("stress_test_status");
    //     scenarioStats.Fail.Request.Percent.Should().BeLessThan(5);
    // }
    
    // [Test]
    // public void Spike()
    // {
    //     var scenario = Scenario.Create("spike_test_status", async context =>
    //         {
    //             using var client = new HttpClient();
    //             var response = await client.GetAsync(Helper.BaseUrl + "/status");
    //             return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
    //         })
    //         .WithLoadSimulations(
    //             Simulation.RampingInject(
    //                 100,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromSeconds(10)
    //             ),
    //             Simulation.Inject(
    //                 100,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromSeconds(10)
    //             ),
    //             Simulation.RampingInject(
    //                 0,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromSeconds(10)
    //             )
    //         );
    //
    //     var stats = NBomberRunner
    //         .RegisterScenarios(scenario)
    //         .Run();
    //     var scenarioStats = stats.ScenarioStats.Get("spike_test_status");
    //     scenarioStats.Fail.Request.Percent.Should().BeLessThan(5);
    // }
    
    // [Test]
    // public void Endurance()
    // {
    //     var scenario = Scenario.Create("endurance_test_status", async context =>
    //         {
    //             using var client = new HttpClient();
    //             var response = await client.GetAsync(Helper.BaseUrl + "/status");
    //             return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
    //         })
    //         .WithLoadSimulations(
    //             Simulation.Inject(
    //                 1,
    //                 TimeSpan.FromSeconds(1),
    //                 TimeSpan.FromHours(1)
    //             )
    //         );
    //
    //     var stats = NBomberRunner
    //         .RegisterScenarios(scenario)
    //         .Run();
    //     var scenarioStats = stats.ScenarioStats.Get("endurance_test_status");
    //     scenarioStats.Fail.Request.Percent.Should().BeLessThan(5);
    // }
    
    
    
}