using infrastructure;
using infrastructure.Mqtt;

namespace service;

public class MotorService(MotorRepository motorRepository, MqttDeviceCommandsRepository mqttRepository)
{
    public int SetMotorPosition(string mac, int position)
    {
        return motorRepository.SetMotorPosition(mac, position);
    }
    
    public async Task<MotorPositionDto> GetMotorPosition(string mac)
    {
        var positions =  motorRepository.GetMotorPosition(mac);
        if (positions.MaxMotorPosition >= positions.LastMotorPosition) return positions;
        positions.MaxMotorPosition = positions.LastMotorPosition;
        await SetMaxMotorPosition(mac, positions.MaxMotorPosition);
        return positions;
    }

    public async Task<int> SetMaxMotorPosition(string mac, int position)
    {
        var max = motorRepository.SetMaxMotorPosition(mac, position);
        await mqttRepository.SendMaxPosition(mac, max);
        return max;
    }

    public bool GetMotorReversed(string mac)
    {
        return motorRepository.GetMotorReversed(mac);
    }

    public async Task<bool> SetMotorDirection(string mac, bool reversed)
    {
        var r = motorRepository.SetMotorReversed(mac, reversed);
        await mqttRepository.SendReverseCommand(mac, reversed);
        return r;
    }
}