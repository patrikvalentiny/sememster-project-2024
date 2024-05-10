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
    
    public int GetMotorPosition(string mac)
    {
        var sql = $@"SELECT last_motor_position
                    FROM climate_ctrl.device_config
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<int>(sql, new { mac });
    }

    public int SetMaxMotorPosition(string mac, int position)
    {
        var sql = $@"UPDATE climate_ctrl.device_config
                    SET max_motor_position = @position
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<int>(sql, new { mac, position });
    }
}