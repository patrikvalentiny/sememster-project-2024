using infrastructure;

namespace service;

public class MotorService(MotorRepository motorRepository)
{
    public int SetMotorPosition(string mac, int position)
    {
        return motorRepository.SetMotorPosition(mac, position);
    }
}