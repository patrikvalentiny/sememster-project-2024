namespace infrastructure.Models;

public class Device
{
    public required int Id { get; init; }
    public required string Mac { get; init; }
    public string? Name { get; init; }
    public int? StatusId { get; init; }
    public string Status { get; init; } = "online";

}