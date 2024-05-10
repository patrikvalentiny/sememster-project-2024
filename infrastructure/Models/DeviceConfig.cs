namespace infrastructure.Models;

public class DeviceConfig
{
    public bool MotorReversed {get; set;} = false;
    public int? LastMotorPosition { get; set; }
    public int? MaxMotorPosition { get; set; }
}