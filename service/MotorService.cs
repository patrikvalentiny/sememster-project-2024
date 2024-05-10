using infrastructure;

namespace service;

public class MotorService(MotorRepository motorRepository)
{
    public int SetMotorPosition(string mac, int position)
    {
        return motorRepository.SetMotorPosition(mac, position);
    }
    
    public MotorPositionDto GetMotorPosition(string mac)
    {
        var positions =  motorRepository.GetMotorPosition(mac);
        if (positions.MaxMotorPosition >= positions.LastMotorPosition) return positions;
        positions.MaxMotorPosition = positions.LastMotorPosition;
        SetMaxMotorPosition(mac, positions.MaxMotorPosition);
        return positions;
    }

    public int SetMaxMotorPosition(string mac, int position)
    {
        return motorRepository.SetMaxMotorPosition(mac, position);
    }
}