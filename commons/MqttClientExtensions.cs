using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace commons;

public static class MqttClientExtensions
{
    public static async Task PublishJsonAsync(this IMqttClient mqttClient, string topic, object? payload)
    {
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var message = new MqttApplicationMessageBuilder()
            .WithTopic((environment == "Development" ? "climatectrl-dev" : "climatectrl") + topic)
            .WithPayload(JsonConvert.SerializeObject(payload, jsonSerializerSettings))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            // .WithRetainFlag()
            .Build();

        await mqttClient.PublishAsync(message);
    }
}