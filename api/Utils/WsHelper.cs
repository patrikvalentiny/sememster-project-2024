using System.Reflection;
using Fleck;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace api.Utils;

public static class WsHelper
{
    private static readonly HashSet<Type> BaseDtos = [];
    public static void InitBaseDtos(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.BaseType != null &&
                type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(BaseDto<>))
            {
                BaseDtos.Add(type);
            }
        }
    }
    
    public static Task InvokeBaseDtoHandler(this IMediator mediator, IWebSocketConnection ws, string message)
    {
        var dto = JsonConvert.DeserializeObject<BaseDto<object>>(message);
        var type = BaseDtos.FirstOrDefault(x => x.Name.Equals(dto!.EventType + "Dto"));
        
        //TODO: exception handling
        if (type == null)
        {
            return Task.CompletedTask;
        }

        var request = JsonConvert.DeserializeObject(message, type);
        //TODO: null check
        var response = mediator.Send(request);
        if (response.Exception != null)
        {
            Log.Error(response.Exception, "Error invoking handler");
            return Task.CompletedTask;
        }
        if (response.Result == null)
        {
            return Task.CompletedTask;
        }
        return ws.SendJson(response.Result);
    }
    
    public static Task SendJson(this IWebSocketConnection ws, object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        Log.Debug("Sending: {json}",json);
        return ws.Send(json);
    }
}