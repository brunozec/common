namespace Brunozec.Common.Repository;

public class DBUoWCreator : IBaseUnitOfWorkFactory
{
    private readonly BaseContext _context;

    public DBUoWCreator(BaseContext context)
    {
        _context = context;
    }

    public async Task<IBaseUnitOfWork> Create()
    {
        var uow = new BaseUnitOfWork(_context);
        await uow.BeginTransaction();
        return uow;
    }
}