using infrastructure;
using infrastructure.Models;

namespace service;

public class DeviceService(DeviceRepository deviceRepository)
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