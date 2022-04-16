namespace Brunozec.Common.Repository;

//https://github.com/zdz72113/NETCore_BasicKnowledge.Examples/blob/master/Documents/1.6%20%5BBasic%5D%20ASP.NET%20Core%20%E4%B8%AD%E7%9A%84%20ORM%20%E4%B9%8B%20Dapper.md
public interface IBaseUnitOfWorkFactory
{
    Task<IBaseUnitOfWork> Create();
}