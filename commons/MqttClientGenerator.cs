using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

namespace commons;

public class MqttClientGenerator

{
    private static MqttFactory _mqttFactory = new MqttFactory();
    public static async Task<IMqttClient> CreateMqttClient(string topic)
    {
        var mqttClient = _mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithCredentials(Environment.GetEnvironmentVariable("ASPNETCORE_Flespi__Username"), "")
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        var connectResult = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            throw new Exception($"Failed to connect to MQTT broker: {connectResult.ResultCode}");

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic((environment == "Development" ? "climatectrl-dev" : "climatectrl") + topic))
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        return mqttClient;
    }

    public static async Task<IMqttClient> CreateMqttClient()
    {
        var mqttClient = _mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithCredentials(Environment.GetEnvironmentVariable("ASPNETCORE_Flespi__Username"), "")
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        var connectResult = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            throw new Exception($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
        
        return mqttClient;
    }

}