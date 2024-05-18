using infrastructure;
using infrastructure.Models;

namespace service;

public interface IConfigService
{
    DeviceConfig? GetDeviceConfig(string mac);
}

public class ConfigService(ConfigRepository configRepository) : IConfigService
{
    public DeviceConfig? GetDeviceConfig(string mac)
    {
        return configRepository.GetDeviceConfig(mac);
    }
}