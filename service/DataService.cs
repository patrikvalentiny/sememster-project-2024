using commons.Models;
using infrastructure;
using infrastructure.Models;

namespace service;

public class DataService(DataRepository dataRepository, WebSocketStateService webSocketStateService)
{
    public BmeDataDto InsertData(BmeData data, string mac)
    {
        return dataRepository.InsertData(data, mac);
    }
}