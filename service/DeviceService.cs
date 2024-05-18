using infrastructure;
using infrastructure.Models;

namespace service;

public interface IDeviceService
{
    Device InsertDevice(string mac);
    IEnumerable<Device> GetDevices();
}


public class DeviceService(DeviceRepository deviceRepository) : IDeviceService
{
    public Device InsertDevice(string mac)
    {
        return deviceRepository.InsertDevice(mac);
    }

    public IEnumerable<Device> GetDevices()
    {
        return deviceRepository.GetDevices();
    }
}