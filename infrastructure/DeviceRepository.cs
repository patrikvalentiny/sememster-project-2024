using System.Data.Common;
using Dapper;

namespace infrastructure;

public class DeviceRepository(DbDataSource dataSource)
{
    public Device InsertDevice(string mac)
    {
        var sql = @$"INSERT INTO climate_ctrl.devices (mac) VALUES (@mac)
                        ON CONFLICT (mac) DO UPDATE SET mac = EXCLUDED.mac
                        RETURNING 
                            id as {nameof(Device.Id)}, 
                            mac as {nameof(Device.Mac)};";
        
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<Device>(sql, new {mac});
    }
}

public record Device(int Id, string Mac);