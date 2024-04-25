using api.Utils;
using infrastructure.Models;

namespace api.ServerEvents;

public class ServerDeviceBmeData : BaseDto
{
    public required BmeDataDto Data { get; init; }
}