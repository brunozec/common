using System.Threading.Tasks;

namespace Brunozec.Common.Specifications.Assertions
{
    public class NotEmptyAssertion : ISpecification<string>
    {
        public virtual Task<bool> IsSatisfiedBy(string value)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(value));
        }
    }
}