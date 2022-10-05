namespace Brunozec.Common.Repository;

public sealed class DBUoWCreator : IBaseUnitOfWorkFactory
{
    private readonly DBContext _context;

    public DBUoWCreator(DBContext context)
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