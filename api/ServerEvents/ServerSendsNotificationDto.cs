using api.Utils;

namespace api.ServerEvents;

public class ServerSendsNotificationDto(string message, NotificationType type = NotificationType.Info): BaseDto
{
    public string Type { get; init; } = type.ToString().ToLower();
    public string? Message { get; init; } = message;
}

public enum NotificationType
{
    Info,
    Error,
    Success,
    Warning
}
