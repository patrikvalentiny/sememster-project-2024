using infrastructure;
using infrastructure.Models;

namespace service;

public class ConfigService(ConfigRepository configRepository)
{
    public DeviceConfig GetDeviceConfig(string mac)
    {
        return configRepository.GetDeviceConfig(mac);
    }
}