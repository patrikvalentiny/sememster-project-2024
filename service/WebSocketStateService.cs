using System.Collections.Concurrent;
using Fleck;

namespace service;

public class WebSocketStateService
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
    public ConcurrentDictionary<string, Guid> MacToConnectionId { get; } = new();
    public ConcurrentDictionary<string, List<Guid>> MotorMacToConnectionId { get; } = new();

}