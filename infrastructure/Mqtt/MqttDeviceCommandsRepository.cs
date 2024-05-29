using commons;
using MQTTnet.Client;

namespace infrastructure.Mqtt;

public class MqttDeviceCommandsRepository
{
    private readonly IMqttClient _mqttClient = MqttClientGenerator.CreateMqttClient().Result;
    public async Task SendReverseCommand(string mac, bool reversed)
    {
        await _mqttClient.PublishJsonAsync($"/devices/{mac}/commands/motor", new { Reversed = reversed });
    }

    public async Task SendMaxPosition(string mac, int position)
    {
        await _mqttClient.PublishJsonAsync($"/devices/{mac}/commands/motor", new { MaxPosition = position });
    }
    
    public async Task SendRtcCommand(string mac, bool start)
    {
        await _mqttClient.PublishJsonAsync($"/devices/{mac}/commands/bmertc", new { Command = start ? "start" : "stop" });
    }
    
}