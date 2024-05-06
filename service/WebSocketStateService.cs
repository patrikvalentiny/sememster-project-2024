using System.Collections.Concurrent;
using Fleck;

namespace service;

public class WebSocketStateService
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
    public ConcurrentDictionary<string, List<Guid>> MacToConnectionId { get; } = new();
    public ConcurrentDictionary<string, List<Guid>> MotorMacToConnectionId { get; } = new();

    public void CloseSocket(IWebSocketConnection socket)
    {
        var id = socket.ConnectionInfo.Id;
        Connections.TryRemove(id, out _);
        foreach (var keyValuePair in MacToConnectionId)
        {
            keyValuePair.Value.Remove(id);
        }
        foreach (var keyValuePair in MotorMacToConnectionId)
        {
            keyValuePair.Value.Remove(id);
        }
    }
}