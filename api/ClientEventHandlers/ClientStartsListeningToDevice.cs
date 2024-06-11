using api.Utils;
using commons.Models;
using MediatR;
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

public class ClientStartsListeningToDevice(IWebSocketStateService stateService, IDataService dataService)
    : IRequestHandler<ClientStartsListeningToDeviceDto, ServerSendsDeviceBaseDataDto>
{
    public Task<ServerSendsDeviceBaseDataDto> Handle(ClientStartsListeningToDeviceDto request,
        CancellationToken cancellationToken)
    {
        var socket = request.Socket!;
        
        // Add the connection id to the set of connection ids for the mac address
        if (stateService.MacToConnectionId.TryGetValue(request.Mac, out var connectionIdList))
            connectionIdList.Add(socket.ConnectionInfo.Id);
        else
            stateService.MacToConnectionId.TryAdd(request.Mac, [socket.ConnectionInfo.Id]);

        // Get the latest data for the mac address
        var data = dataService.GetLatestData(request.Mac);
        // Send the data to the client
        return Task.FromResult(new ServerSendsDeviceBaseDataDto { Mac = request.Mac, Data = data });
    }
}