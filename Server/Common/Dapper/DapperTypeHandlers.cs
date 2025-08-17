using System.Data;
using Dapper;

// Dapper seems to be unable to handle DateOnly and TimeOnly so we need to manually configure how this works for now
// See: https://github.com/DapperLib/Dapper/issues/2071

namespace Server.Common.Dapper;

/// <summary>
/// Workaround for Dapper not handling DateOnly
/// </summary>
public class DapperSqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly date)
        => parameter.Value = date.ToDateTime(new TimeOnly(0, 0));

    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime((DateTime)value);
}

/// <summary>
/// Workaround for Dapper not handling TimeOnly
/// </summary>
public class DapperSqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly time)
    {
        parameter.Value = time.ToString();
    }

    public override TimeOnly Parse(object value) => TimeOnly.FromTimeSpan((TimeSpan)value);
}