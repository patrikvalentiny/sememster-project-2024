using infrastructure;

namespace service;

public class DeviceService(DeviceRepository deviceRepository)
{
    public TestRecord StatusCheck()
    {
        return deviceRepository.InsertDevice("test");
    }
}