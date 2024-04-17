using System.Reflection;

namespace api.Utils;

public class WsHelper
{
    public static HashSet<Type> GetBaseDtos(Assembly assembly)
    {
        var baseDtos = new HashSet<Type>();
        
        foreach (var type in assembly.GetTypes())
        {
            if (type.BaseType != null &&
                type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(BaseDto<>))
            {
                baseDtos.Add(type);
            }
        }

        return baseDtos;
    }
}