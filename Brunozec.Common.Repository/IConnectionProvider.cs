using System.Data;

namespace Brunozec.Common.Repository;

public interface IConnectionProvider
{
    Task<IDbConnection> CreateConnection();
}