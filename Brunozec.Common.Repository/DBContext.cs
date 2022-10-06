using System.Data;
using System.Data.SqlClient;
using Brunozec.Dapper.Dommel;

namespace Brunozec.Common.Repository;

public sealed class DBContext : BaseContext
{
    private readonly IConnectionProvider _connectionProvider;

    public DBContext(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    protected override IDbConnection CreateConnection()
    {
        return _connectionProvider.CreateConnection();
    }

    protected override Task CreateConfiguration()
    {
        DommelMapper.AddSqlBuilder(typeof(SqlConnection), new SqlServerSqlBuilder());

        return Task.CompletedTask;
    }
}