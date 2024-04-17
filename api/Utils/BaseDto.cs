namespace api.Utils;

public class BaseDto
{
    protected BaseDto()
    {
        var eventType = GetType().Name;
        var subString = eventType.Substring(eventType.Length - 3);
        EventType = subString.ToLower().Equals("dto") ? eventType.Substring(0, eventType.Length - 3) : eventType;
    }

    public string EventType { get; init;}
}