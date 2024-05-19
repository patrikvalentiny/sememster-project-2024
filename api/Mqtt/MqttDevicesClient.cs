using api.ServerEvents;
using api.Utils;
using commons;
using infrastructure.Models;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttDevicesClient(
    IWebSocketStateService webSocketStateService,
    IDeviceService deviceService,
    IConfigService configService)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient("/devices");

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var m = e.ApplicationMessage;
                var topic = m.Topic;
                var message = m.ConvertPayloadToString();
                Log.Debug("Mqtt Message received: {Message}, Topic: {Topic}", message, topic);
                var payload = JsonConvert.DeserializeObject<Device>(message)!;
                var device = deviceService.InsertDevice(payload.Mac);

                var config = configService.GetDeviceConfig(device.Mac);

                webSocketStateService.Connections.Values.ToList().ForEach(async socket =>
                {
                    try
                    {
                        await socket.SendJson(new ServerDeviceOnline { Device = device });
                        if (config == null)
                            await socket.SendWarning($"Device {device.Mac} is online and needs to be setup");
                    }
                    catch (Exception exc)
                    {
                        Log.Error(exc, "Error sending message to client");
                    }
                });
                if (config != null)
                    await mqttClient.PublishJsonAsync($"/devices/{device.Mac}/config", config);
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error handling message");
            }
        };
    }
}