using System.Collections.Concurrent;
using System.Reflection;
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


        if (dto == null) throw new NullReferenceException("Could not deserialize message");

        var eventType = (dto.EventType.EndsWith("dto", StringComparison.OrdinalIgnoreCase)
            ? dto.EventType.Substring(0, dto.EventType.Length - 3)
            : dto.EventType).ToLower();


        if (!BaseDtos.TryGetValue(eventType, out var type)) throw new NullReferenceException("Could not find type");

        var request = JsonConvert.DeserializeObject(message, type);
        //TODO: null check
        var response = mediator.Send(request);
        if (response.Exception != null)
        {
            Log.Error(response.Exception, "Error invoking handler");
            return Task.CompletedTask;
        }

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
}