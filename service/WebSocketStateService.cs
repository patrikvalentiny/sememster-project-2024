using System.Collections.Concurrent;
using Fleck;

namespace service;

public class WebSocketStateService
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
}