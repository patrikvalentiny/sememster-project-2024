using infrastructure;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttClientService(WebSocketStateService webSocketStateService, DeviceService deviceService)
{
     public async Task CommunicateWithBroker()
    {

        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithCredentials(Environment.GetEnvironmentVariable("ASPNETCORE_Flespi__Username"), "")
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic(environment == "Development" ? "climatectrl-dev/#" : "climatectrl/#"))
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var m = e.ApplicationMessage;
                var topic = m.Topic;
                var topicList = topic.Split("/");
                var message = m.ConvertPayloadToString();
                Log.Debug("Mqtt Message received: {Message}, Topic: {Topic}", message, topic);
                if (topicList[1] == "devices" && topicList.Length == 2)
                {
                    deviceService.InsertDevice(JsonConvert.DeserializeObject<Device>(message)!.Mac);
                }
                
                webSocketStateService.Connections.Values.ToList().ForEach(async socket =>
                {
                    try
                    {
                        await socket.Send(message);
                    } catch (Exception exc)
                    {
                        Log.Error(exc, "Error sending message to client");
                    }
                });
                
                
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error handling message");
            }
        };
    
    }
}