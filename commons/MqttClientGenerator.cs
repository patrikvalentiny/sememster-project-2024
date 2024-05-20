using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Serilog;

namespace commons;

public static class MqttClientGenerator

{
    private static readonly MqttFactory MqttFactory = new();

    public static async Task<IMqttClient> CreateMqttClient(string topic)
    {
        var mqttClient = await CreateMqttClient();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f =>
                f.WithTopic((environment == "Development" ? "climatectrl-dev" : "climatectrl") + topic))
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        return mqttClient;
    }

    public static async Task<IMqttClient> CreateMqttClient()
    {
        var mqttClient = MqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithCredentials(Environment.GetEnvironmentVariable("ASPNETCORE_Flespi__Username"), "")
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        try
        {
            var connectResult = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
                throw new MqttConnectingFailedException($"Failed to connect to MQTT broker: {connectResult.ResultCode}",
                    new HttpRequestException(), connectResult);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }


        return mqttClient;
    }
}