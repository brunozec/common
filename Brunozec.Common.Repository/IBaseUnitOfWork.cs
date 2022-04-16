namespace Brunozec.Common.Repository;

public interface IBaseUnitOfWork:IAsyncDisposable
{
    Task CommitAsync();
}