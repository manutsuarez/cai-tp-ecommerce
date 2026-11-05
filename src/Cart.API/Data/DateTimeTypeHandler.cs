using System.Data;
using System.Globalization;
using Dapper;

namespace Cart.API.Data;

/// <summary>
/// Las fechas se guardan como texto ISO 8601 en UTC ("2024-03-10T10:45:00.0000000Z").
/// Este handler hace que al leerlas sigan en UTC (con "Z"), como pide la consigna.
/// </summary>
public class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value.ToUniversalTime().ToString("O");
    }

    public override DateTime Parse(object value)
    {
        return DateTime.Parse((string)value, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }
}