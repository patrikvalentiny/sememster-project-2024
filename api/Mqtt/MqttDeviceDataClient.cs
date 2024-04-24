using api.Mqtt.Helpers;

namespace api.Mqtt;

public class MqttDeviceDataClient(MqttClientGenerator clientGenerator)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await clientGenerator.CreateMqttClient("/data/+/bme280");
        
    }

}