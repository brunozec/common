namespace Brunozec.Common.Repository;

public sealed class DBUoWCreator : IBaseUnitOfWorkFactory
{
    private readonly DBContext _context;

    public DBUoWCreator(DBContext context)
    {
        _context = context;
    }

    public IBaseUnitOfWork Create()
    {
        return new BaseUnitOfWork(_context);
    }
}