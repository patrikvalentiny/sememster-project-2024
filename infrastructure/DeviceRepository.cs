using System.Data.Common;
using Dapper;

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
                            status_id as {nameof(Device.StatusId)}";

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
                            FROM climate_ctrl.devices d INNER JOIN climate_ctrl.device_status ds on ds.id = d.status_id";
        using var conn = dataSource.OpenConnection();
        return conn.Query<Device>(sql);
    }
}

public class Device
{
    public required int Id { get; init; }
    public required string Mac { get; init; }
    public string? Name { get; init; }
    public int? StatusId { get; init; }
    public string Status { get; init; } = "online";

}