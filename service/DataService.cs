using commons.Models;
using infrastructure;
using infrastructure.Models;
using infrastructure.Mqtt;

namespace service;

public interface IDataService
{
    BmeDataDto InsertData(BmeData data, string mac);
    IEnumerable<BmeData> GetLatestData(string requestMac);
    IEnumerable<BmeData> GetDeviceDataInLastXDays(string requestMac, int days);
    Task StartRtc(string requestMac, Guid connectionInfoId);
    Task StopRtc(string requestMac, Guid connectionInfoId);
}

public class DataService(DataRepository dataRepository, IWebSocketStateService stateService, MqttDeviceCommandsRepository mqtt) : IDataService
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

    public async Task StartRtc(string requestMac, Guid connectionInfoId)
    {
        if(stateService.RtcMacToConnectionId.TryGetValue(requestMac, out var set))
        {
            set.Add(connectionInfoId);
        }
        else
        {
            stateService.RtcMacToConnectionId.TryAdd(requestMac, [connectionInfoId]);
            await mqtt.SendRtcCommand(requestMac, true);
        }
    }
    
    public async Task StopRtc(string requestMac, Guid connectionInfoId)
    {
        if(stateService.RtcMacToConnectionId.TryGetValue(requestMac, out var set))
        {
            set.Remove(connectionInfoId);
            if (set.Count == 0)
            {
                stateService.RtcMacToConnectionId.TryRemove(requestMac, out _);
                await mqtt.SendRtcCommand(requestMac, false);
            }
        }
    }
}