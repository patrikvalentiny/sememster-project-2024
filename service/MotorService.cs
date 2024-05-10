using infrastructure;

namespace service;

public class MotorService(MotorRepository motorRepository)
{
    public int SetMotorPosition(string mac, int position)
    {
        return motorRepository.SetMotorPosition(mac, position);
    }
    
    public int GetMotorPosition(string mac)
    {
        return motorRepository.GetMotorPosition(mac);
    }

    public int SetMaxMotorPosition(string mac, int position)
    {
        return motorRepository.SetMaxMotorPosition(mac, position);
    }
}