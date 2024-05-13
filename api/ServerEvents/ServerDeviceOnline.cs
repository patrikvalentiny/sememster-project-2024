using api.Utils;
using infrastructure.Models;

namespace api.ServerEvents;

public class ServerDeviceOnline : BaseDto
{
    public required Device Device { get; init; }
}