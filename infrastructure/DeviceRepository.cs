using System.Data.Common;
using Dapper;
using infrastructure.Models;

namespace infrastructure;

public class DeviceRepository(DbDataSource dataSource)
{
    public Device InsertDevice(string mac)
    {
        var sql = @$"INSERT INTO climate_ctrl.devices (mac) VALUES (@mac)
                        ON CONFLICT (mac) DO UPDATE SET status_id = 1
                        RETURNING 
                            id as {nameof(Device.Id)}, 
                            mac as {nameof(Device.Mac)},
                            device_name as {nameof(Device.Name)},
                            status_id as {nameof(Device.StatusId)};
                INSERT INTO climate_ctrl.device_config (mac, last_motor_position, max_motor_position) VALUES (@mac, 0, 1000)
                        ON CONFLICT (mac) DO NOTHING;";

        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<Device>(sql, new { mac });
    }

    public IEnumerable<Device> GetDevices()
    {
        var sql = @$"SELECT d.id as {nameof(Device.Id)},
                            d.mac as {nameof(Device.Mac)},
                            d.device_name as {nameof(Device.Name)},
                            d.status_id as {nameof(Device.StatusId)},
                            ds.value as {nameof(Device.Status)}
                            FROM climate_ctrl.devices d INNER JOIN climate_ctrl.device_status ds on ds.id = d.status_id ORDER BY d.id";
        using var conn = dataSource.OpenConnection();
        return conn.Query<Device>(sql);
    }
}