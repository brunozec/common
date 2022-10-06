using System.Data;

namespace Brunozec.Common.Repository;

public interface IConnectionProvider
{
    IDbConnection CreateConnection();
}