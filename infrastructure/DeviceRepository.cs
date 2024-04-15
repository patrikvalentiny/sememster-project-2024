using System.Data.Common;
using Dapper;

namespace infrastructure;

public class DeviceRepository(DbDataSource dataSource)
{
    public TestRecord InsertDevice(string mac)
    {
        var sql = @$"INSERT INTO climate_ctrl.devices (mac) VALUES (@mac) 
                        RETURNING 
                            id as {nameof(TestRecord.Id)}, 
                            mac as {nameof(TestRecord.Mac)};";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<TestRecord>(sql, mac);
    }
}

public record TestRecord(int Id, string Mac);