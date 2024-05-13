using api.Utils;
using commons.Models;
using infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.ClientEventHandlers;

public class ClientStartsListeningToDeviceDto : BaseDto, IRequest<ServerSendsDeviceBaseDataDto>
{
    public required string Mac { get; set; }
}

public class ServerSendsDeviceBaseDataDto : BaseDto
{
    public required string Mac { get; set; }
    public required IEnumerable<BmeData> Data { get; set; }
}

public class ClientStartsListeningToDevice(WebSocketStateService stateService, DataService dataService) : IRequestHandler<ClientStartsListeningToDeviceDto, ServerSendsDeviceBaseDataDto>
{
    public Task<ServerSendsDeviceBaseDataDto> Handle(ClientStartsListeningToDeviceDto request, CancellationToken cancellationToken)
    {
        var socket = request.Socket!;
        
        if (stateService.MacToConnectionId.TryGetValue(request.Mac, out var connectionIdList))
        {
            connectionIdList.Add(socket.ConnectionInfo.Id);
        }
        else
        {
            stateService.MacToConnectionId.TryAdd(request.Mac, [socket.ConnectionInfo.Id]);
        }
        
        foreach (var keyValuePair in stateService.MacToConnectionId)
        {
            Log.Debug("Mac: {Mac}, ConnectionId: {ConnectionId}", keyValuePair.Key, keyValuePair.Value);
        }
        var data = dataService.GetLastData(request.Mac);
        return Task.FromResult(new ServerSendsDeviceBaseDataDto { Mac = request.Mac, Data = data });
    }
}