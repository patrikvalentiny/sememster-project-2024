using api.Utils;
using MediatR;

namespace api.ClientEventHandlers;

public class ClientSaysHelloDto : BaseDto, IRequest<ServerSaysHelloDto>
{
    public required string Message { get; set; }
}

public class ServerSaysHelloDto : BaseDto
{
    public required string Message { get; set; }
}

public class ClientSaysHelloHandler : IRequestHandler<ClientSaysHelloDto, ServerSaysHelloDto>
{
    public Task<ServerSaysHelloDto> Handle(ClientSaysHelloDto request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ServerSaysHelloDto { Message = $"Hello, {request.Message}!" });
    }
}