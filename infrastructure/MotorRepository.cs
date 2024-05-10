using System.Data.Common;
using Dapper;

namespace infrastructure;

public class MotorRepository(DbDataSource dataSource)
{
    public int SetMotorPosition(string mac, int position)
    {
        var sql = $@"UPDATE climate_ctrl.device_config
                    SET last_motor_position = @position
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.Execute(sql, new { mac, position });
    }
}