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

    public static async Task InvokeBaseDtoHandler(this IWebSocketConnection ws, string message, IMediator mediator)
    {
        if (BaseDtos.IsEmpty) InitBaseDtos(Assembly.GetExecutingAssembly());
        var dto = JsonConvert.DeserializeObject<BaseDto>(message, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
        });

        if (dto == null) throw new ArgumentException("Could not deserialize message",message);

        // Remove the "dto" suffix from the event type and convert to lowercase
        var eventType = (dto.EventType.EndsWith("dto", StringComparison.OrdinalIgnoreCase)
            ? dto.EventType.Substring(0, dto.EventType.Length - 3)
            : dto.EventType).ToLower();


        // Get the type from the dictionary
        if (!BaseDtos.TryGetValue(eventType, out var type)) throw new ArgumentNullException(dto.EventType, "Event type not found");

        // Deserialize the message to the type
        var request = JsonConvert.DeserializeObject(message, type)!;

        // Set the socket property
        if (request is BaseDto baseDto) baseDto.Socket = ws;

        // Send the request to the mediator
        var response = await mediator.Send(request);
        // If the response is null, return a completed task otherwise send the response
        if (response!.GetType() != Unit.Value.GetType())
            await ws.SendJson(response);
    }

    public static Task SendJson<T>(this IWebSocketConnection ws, T obj)
    {
        var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        Log.Debug("Sending: {json}", json);
        return ws.Send(json);
    }


    public static async Task SendNotification(this IWebSocketConnection socket, string message)
    {
        await socket.SendJson(new ServerSendsNotificationDto(message));
    }

    public static async Task SendError(this IWebSocketConnection socket, string message)
    {
        await socket.SendJson(new ServerSendsNotificationDto(message, NotificationType.Error));
    }

    public static async Task SendSuccess(this IWebSocketConnection socket, string message)
    {
        await socket.SendJson(new ServerSendsNotificationDto(message, NotificationType.Success));
    }

    public static async Task SendWarning(this IWebSocketConnection socket, string message)
    {
        await socket.SendJson(new ServerSendsNotificationDto(message, NotificationType.Warning));
    }

    public static async Task Handle(this Exception ex, IWebSocketConnection ws)
    {
        await ws.SendError(ex.Message);
        Log.Error(ex, ex.Message);
    }

    public class HandlerException(string message) : Exception(message);
}