using System.Threading.Tasks;

namespace Brunozec.Common.Specifications
{
    public interface ISpecification<TEntity>
    {
        Task<bool> IsSatisfiedBy(TEntity entity);
    }
}