using System;
using System.Threading.Tasks;

namespace Brunozec.Common.Specifications
{
    public class NotSpecification<TEntity> : ISpecification<TEntity>
    {
        public NotSpecification(ISpecification<TEntity> spec)
        {
            Wrapped = spec ?? throw new ArgumentNullException(nameof(spec));
        }

        protected ISpecification<TEntity> Wrapped { get; }

        public async Task<bool> IsSatisfiedBy(TEntity entity)
        {
            return !await Wrapped.IsSatisfiedBy(entity);
        }
    }
}