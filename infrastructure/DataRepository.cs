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

    public IEnumerable<BmeData> GetLatestData(string requestMac)
    {
        var sql = $@"SELECT 
                        temperature_c as {nameof(BmeData.TemperatureC)},
                        humidity as {nameof(BmeData.Humidity)},
                        pressure as {nameof(BmeData.Pressure)},
                        utc_time as {nameof(BmeData.CreatedAt)}
                    FROM climate_ctrl.bme_data
                    WHERE device_mac = @requestMac and utc_time >  (now() at time zone 'utc') - interval '1 day'
                    ORDER BY utc_time DESC
                    LIMIT 25";
        using var conn = dataSource.OpenConnection();
        return conn.Query<BmeData>(sql, new { requestMac });
    }
}