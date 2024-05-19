using api.Utils;
using commons.Models;
using MediatR;
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

public class ClientStartsListeningToDevice(IWebSocketStateService stateService, IDataService dataService)
    : IRequestHandler<ClientStartsListeningToDeviceDto, ServerSendsDeviceBaseDataDto>
{
    public Task<ServerSendsDeviceBaseDataDto> Handle(ClientStartsListeningToDeviceDto request,
        CancellationToken cancellationToken)
    {
        var socket = request.Socket!;

        if (stateService.MacToConnectionId.TryGetValue(request.Mac, out var connectionIdList))
            connectionIdList.Add(socket.ConnectionInfo.Id);
        else
            stateService.MacToConnectionId.TryAdd(request.Mac, [socket.ConnectionInfo.Id]);
        
        var data = dataService.GetLatestData(request.Mac);
        return Task.FromResult(new ServerSendsDeviceBaseDataDto { Mac = request.Mac, Data = data });
    }
}