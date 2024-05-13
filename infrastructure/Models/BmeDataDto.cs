namespace infrastructure.Models;

public class BmeDataDto
{
    public required int Id { get; init; }
    public required string DeviceMac { get; init; }
    public required float TemperatureC { get; init; }
    public required float Humidity { get; init; }
    public required float Pressure { get; init; }
    public required DateTime CreatedAt { get; set; }
}