using commons.Models;
using infrastructure;
using infrastructure.Models;

namespace service;

public interface IDataService
{
    BmeDataDto InsertData(BmeData data, string mac);
    IEnumerable<BmeData> GetLatestData(string requestMac);
    IEnumerable<BmeData> GetDeviceDataInLastXDays(string requestMac, int days);
}

public class DataService(DataRepository dataRepository) : IDataService
{
    public BmeDataDto InsertData(BmeData data, string mac)
    {
        return dataRepository.InsertData(data, mac);
    }

    public IEnumerable<BmeData> GetLatestData(string requestMac)
    {
        return dataRepository.GetLatestData(requestMac);
    }

    public IEnumerable<BmeData> GetDeviceDataInLastXDays(string requestMac, int days)
    {
        return dataRepository.GetDeviceDataInLastXDays(requestMac, days);
    }
}