namespace api.Utils;

public class BaseDto<T>
{
    protected BaseDto()
    {
        var eventType = typeof(T).Name;
        var subString = eventType.Substring(eventType.Length - 3);
        EventType = subString.ToLower().Equals("dto") ? eventType.Substring(0, eventType.Length - 3) : eventType;
    }

    public string EventType { get; set; }
}