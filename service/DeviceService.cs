using infrastructure;
using infrastructure.Models;

namespace service;

public interface IDeviceService
{
    Device InsertDevice(string mac);
    IEnumerable<Device> GetDevices();
    DeviceConfig? GetDeviceConfig(string mac);
}

public class DeviceService(DeviceRepository deviceRepository, ConfigRepository configRepository) : IDeviceService
{
    public Device InsertDevice(string mac)
    {
        return deviceRepository.InsertDevice(mac);
    }

    public IEnumerable<Device> GetDevices()
    {
        return deviceRepository.GetDevices();
    }

    public DeviceConfig? GetDeviceConfig(string mac)
    {
        return configRepository.GetDeviceConfig(mac);
    }
}