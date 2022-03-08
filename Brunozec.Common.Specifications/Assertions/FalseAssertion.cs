using System.Threading.Tasks;

namespace Brunozec.Common.Specifications.Assertions
{
    public class FalseAssertion : ISpecification<bool>
    {
        public virtual Task<bool> IsSatisfiedBy(bool value)
        {
            return Task.FromResult(!value);
        }
    }
}