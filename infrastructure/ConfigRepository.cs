using System.Data.Common;
using Dapper;
using infrastructure.Models;

namespace infrastructure;

public class ConfigRepository(DbDataSource dataSource)
{
    public DeviceConfig? GetDeviceConfig(string mac)
    {
        var sql = $@"SELECT 
                        last_motor_position as {nameof(DeviceConfig.LastMotorPosition)},
                        max_motor_position as {nameof(DeviceConfig.MaxMotorPosition)},
                        motor_reversed as {nameof(DeviceConfig.MotorReversed)}
                    FROM climate_ctrl.device_config
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirstOrDefault<DeviceConfig>(sql, new { mac });
    }
}