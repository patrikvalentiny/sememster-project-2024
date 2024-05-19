using System.Data.Common;
using Dapper;

namespace infrastructure;

public class MotorRepository(DbDataSource dataSource)
{
    public int SetMotorPosition(string mac, int position)
    {
        var sql = @"UPDATE climate_ctrl.device_config
                    SET last_motor_position = @position
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.Execute(sql, new { mac, position });
    }

    public MotorPositionDto GetMotorPosition(string mac)
    {
        var sql = $@"SELECT 
                    last_motor_position as {nameof(MotorPositionDto.LastMotorPosition)}, 
                    max_motor_position as {nameof(MotorPositionDto.MaxMotorPosition)},
                    motor_reversed as {nameof(MotorPositionDto.MotorReversed)}
                    FROM climate_ctrl.device_config
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<MotorPositionDto>(sql, new { mac });
    }

    public int SetMaxMotorPosition(string mac, int position)
    {
        var sql = @"UPDATE climate_ctrl.device_config
                    SET max_motor_position = @position
                    WHERE mac = @mac
                    RETURNING max_motor_position";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<int>(sql, new { mac, position });
    }

    public bool SetMotorReversed(string mac, bool reversed)
    {
        var sql = @"UPDATE climate_ctrl.device_config
                    SET motor_reversed = @reversed
                    WHERE mac = @mac
                    RETURNING motor_reversed";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<bool>(sql, new { mac, reversed });
    }

    public bool GetMotorReversed(string mac)
    {
        var sql = @"SELECT motor_reversed
                    FROM climate_ctrl.device_config
                    WHERE mac = @mac";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<bool>(sql, new { mac });
    }
}

public class MotorPositionDto
{
    public int LastMotorPosition { get; init; }
    public int MaxMotorPosition { get; set; }
    public bool MotorReversed { get; init; }
}