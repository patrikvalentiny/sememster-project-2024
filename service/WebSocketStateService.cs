using System.Collections.Concurrent;
using Fleck;

namespace service;

public interface IWebSocketStateService
{
    ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; }
    ConcurrentDictionary<string, HashSet<Guid>> MacToConnectionId { get; }
    ConcurrentDictionary<string, HashSet<Guid>> MotorMacToConnectionId { get; }
    void CloseSocket(IWebSocketConnection socket);
}

public class WebSocketStateService : IWebSocketStateService
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
    public ConcurrentDictionary<string, HashSet<Guid>> MacToConnectionId { get; } = new();
    public ConcurrentDictionary<string, HashSet<Guid>> MotorMacToConnectionId { get; } = new();

    public void CloseSocket(IWebSocketConnection socket)
    {
        var id = socket.ConnectionInfo.Id;
        Connections.TryRemove(id, out _);
        foreach (var keyValuePair in MacToConnectionId) keyValuePair.Value.Remove(id);
        foreach (var keyValuePair in MotorMacToConnectionId) keyValuePair.Value.Remove(id);
    }
}