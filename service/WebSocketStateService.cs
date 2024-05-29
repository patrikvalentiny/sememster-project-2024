using System.Collections.Concurrent;
using System.Text.Json;
using Fleck;
using infrastructure.Mqtt;
using Serilog;

namespace service;

public interface IWebSocketStateService
{
    ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; }
    ConcurrentDictionary<string, HashSet<Guid>> MacToConnectionId { get; }
    ConcurrentDictionary<string, HashSet<Guid>> MotorMacToConnectionId { get; }
    ConcurrentDictionary<string, HashSet<Guid>> RtcMacToConnectionId { get; }
    Task CloseSocket(IWebSocketConnection socket);
}

public class WebSocketStateService(MqttDeviceCommandsRepository mqtt) : IWebSocketStateService
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
    public ConcurrentDictionary<string, HashSet<Guid>> MacToConnectionId { get; } = new();
    public ConcurrentDictionary<string, HashSet<Guid>> MotorMacToConnectionId { get; } = new();
    public ConcurrentDictionary<string, HashSet<Guid>> RtcMacToConnectionId { get; } = new();

    public async Task CloseSocket(IWebSocketConnection socket)
    {
        var id = socket.ConnectionInfo.Id;
        Connections.TryRemove(id, out _);
        foreach (var keyValuePair in MacToConnectionId) keyValuePair.Value.Remove(id);
        foreach (var keyValuePair in MotorMacToConnectionId) keyValuePair.Value.Remove(id);
        // stop rtc for if the connection is the last one
        foreach (var keyValuePair in RtcMacToConnectionId)
        {
            keyValuePair.Value.Remove(id);
            if (keyValuePair.Value.Count != 0) continue;
            RtcMacToConnectionId.TryRemove(keyValuePair.Key, out _);
            await mqtt.SendRtcCommand(keyValuePair.Key, false);
        }
    }
}