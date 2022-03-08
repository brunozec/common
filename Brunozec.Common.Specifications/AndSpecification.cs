using System;
using System.Threading.Tasks;

namespace Brunozec.Common.Specifications
{
    public class AndSpecification<TEntity> : ISpecification<TEntity>
    {
        public AndSpecification(ISpecification<TEntity> spec1, ISpecification<TEntity> spec2)
        {
            Spec1 = spec1 ?? throw new ArgumentNullException(nameof(spec1));
            Spec2 = spec2 ?? throw new ArgumentNullException(nameof(spec2));
        }

        protected ISpecification<TEntity> Spec1 { get; }

        protected ISpecification<TEntity> Spec2 { get; }

        public async Task<bool> IsSatisfiedBy(TEntity entity)
        {
            return await Spec1.IsSatisfiedBy(entity) && await Spec2.IsSatisfiedBy(entity);
        }
    }
}