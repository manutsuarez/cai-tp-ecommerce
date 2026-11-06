using System.Data;
using Dapper;

namespace Orders.API.Data;

/// <summary>
/// SQLite no tiene tipo Guid: los IDs se guardan como texto.
/// Este handler le explica a Dapper cómo pasar de texto a Guid y al revés.
/// </summary>
public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        return Guid.Parse((string)value);
    }
}