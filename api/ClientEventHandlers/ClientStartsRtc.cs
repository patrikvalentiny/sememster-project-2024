using api.ServerEvents;
using api.Utils;
using MediatR;
using service;

namespace api.ClientEventHandlers;

public class ClientStartsRtc : BaseDto, IRequest<ServerSendsNotificationDto>
{
    public required string mac { get; set; }
}

public class ClientStopsRtc : BaseDto, IRequest<ServerSendsNotificationDto>
{
    public required string mac { get; set; }
}

public class ClientStartsRtcHandler(IDataService dataService) : IRequestHandler<ClientStartsRtc, ServerSendsNotificationDto>
{
    public async Task<ServerSendsNotificationDto> Handle(ClientStartsRtc request, CancellationToken cancellationToken)
    {
        await dataService.StartRtc(request.mac, request.Socket!.ConnectionInfo.Id);
        return new ServerSendsNotificationDto("RTC started", NotificationType.Success);
    }
}

public class ClientStopsRtcHandler(IDataService dataService) : IRequestHandler<ClientStopsRtc, ServerSendsNotificationDto>
{
    public async Task<ServerSendsNotificationDto> Handle(ClientStopsRtc request, CancellationToken cancellationToken)
    {
        await dataService.StopRtc(request.mac, request.Socket!.ConnectionInfo.Id);
        return new ServerSendsNotificationDto("RTC stopped", NotificationType.Success);

    }
}