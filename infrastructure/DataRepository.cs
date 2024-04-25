using System.Data.Common;
using commons.Models;
using Dapper;
using infrastructure.Models;

namespace infrastructure;

public class DataRepository(DbDataSource dataSource)
{
    public BmeDataDto InsertData(BmeData data, string mac)
    {
        var sql = $@"INSERT INTO climate_ctrl.bme_data (device_mac, temperature_c, humidity, pressure) 
                        VALUES (@mac, @temperatureC, @humidity, @pressure)
                        RETURNING 
                            id as {nameof(BmeDataDto.Id)}, 
                            device_mac as {nameof(BmeDataDto.DeviceMac)},
                            temperature_c as {nameof(BmeDataDto.TemperatureC)},
                            humidity as {nameof(BmeDataDto.Humidity)},
                            pressure as {nameof(BmeDataDto.Pressure)},
                            utc_time as {nameof(BmeDataDto.CreatedAt)}";
        using var conn = dataSource.OpenConnection();
        return conn.QueryFirst<BmeDataDto>(sql, new { mac, data.TemperatureC, data.Humidity, data.Pressure });
    }
}