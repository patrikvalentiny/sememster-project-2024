using System.Collections.Concurrent;
using System.Reflection;
using api.ServerEvents;
using Fleck;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace api.Utils;

public static class WsHelper
{
    private static readonly ConcurrentDictionary<string, Type> BaseDtos = [];

    public static void InitBaseDtos(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
            if (type.BaseType != null
                && type.BaseType == typeof(BaseDto))
            {
                var eventType = (type.Name.ToLower().EndsWith("dto")
                    ? type.Name.Substring(0, type.Name.Length - 3)
                    : type.Name).ToLower();
                BaseDtos.TryAdd(eventType, type);
            }
    }

    public static Task InvokeBaseDtoHandler(this IWebSocketConnection ws, string message, IMediator mediator)
    {
        var dto = JsonConvert.DeserializeObject<BaseDto>(message, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        
        if (dto == null) throw new NullReferenceException("Could not deserialize BaseDto");

        // Remove the "dto" suffix from the event type and convert to lowercase
        var eventType = (dto.EventType.EndsWith("dto", StringComparison.OrdinalIgnoreCase)
            ? dto.EventType.Substring(0, dto.EventType.Length - 3)
            : dto.EventType).ToLower();


        // Get the type from the dictionary
        if (!BaseDtos.TryGetValue(eventType, out var type)) throw new NullReferenceException("Could not find type");
        
        // Deserialize the message to the type
        var request = JsonConvert.DeserializeObject(message, type);

        // Set the socket property
        switch (request)
        {
            case null:
                throw new NullReferenceException("Could not deserialize to type");
            case BaseDto baseDto:
                baseDto.Socket = ws;
                break;
        }
        
        // Send the request to the mediator
        var response = mediator.Send(request);
        if (response.Exception != null) throw new HandlerException(response.Exception.Message);
        // If the response is null, return a completed task otherwise send the response
        return response.Result == null ? Task.CompletedTask : ws.SendJson(response.Result);
    }

    private static Task SendJson<T>(this IWebSocketConnection ws, T obj)
    {
        var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        Log.Debug("Sending: {json}", json);
        return ws.Send(json);
    }
    

    public static void SendNotification(this IWebSocketConnection socket, string message)
    {
        socket.SendJson(new ServerSendsNotificationDto(message));
    }

    public static void SendError(this IWebSocketConnection socket, string message)
    {
        socket.SendJson(new ServerSendsNotificationDto(message, NotificationType.Error));
    }

    public static void SendSuccess(this IWebSocketConnection socket, string message)
    {
        socket.SendJson(new ServerSendsNotificationDto(message, NotificationType.Success));
    }

    public static void SendWarning(this IWebSocketConnection socket, string message)
    {
        socket.SendJson(new ServerSendsNotificationDto(message, NotificationType.Warning));
    }

    public static void Handle(this Exception ex, IWebSocketConnection ws)
    {
        ws.SendError(ex.Message);
        Log.Error(ex, ex.Message);
    }

    public class HandlerException(string message) : Exception(message);
}