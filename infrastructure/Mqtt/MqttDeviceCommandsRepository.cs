using commons;

namespace infrastructure.Mqtt;

public static class MqttDeviceCommandsRepository
{
    public static async Task SendReverseCommand(string mac, bool reversed)
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient();
        await mqttClient.PublishJsonAsync($"/devices/{mac}/commands/motor", new { Reversed = reversed });
        mqttClient.Dispose();
    }

    public static async Task SendMaxPosition(string mac, int position)
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient();
        await mqttClient.PublishJsonAsync($"/devices/{mac}/commands/motor", new { MaxPosition = position });
        mqttClient.Dispose();
    }
}