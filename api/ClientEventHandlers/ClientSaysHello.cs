using api.Utils;
using MediatR;

namespace api.ClientEventHandlers;

public class ClientSaysHelloDto : BaseDto<ClientSaysHelloDto>, IRequest<string>
{
    public string Message { get; set; }
}

public class ClientSaysHelloHandler : IRequestHandler<ClientSaysHelloDto, string>
{
    public Task<string> Handle(ClientSaysHelloDto request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Message);
    }
}