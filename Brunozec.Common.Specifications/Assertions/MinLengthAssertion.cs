using System.Threading.Tasks;

namespace Brunozec.Common.Specifications.Assertions
{
    public class MinLengthAssertion : ISpecification<string>
    {
        private readonly int _minimum;

        public MinLengthAssertion(int minimum)
        {
            _minimum = minimum;
        }

        public virtual Task<bool> IsSatisfiedBy(string value)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(value) && value.Trim().Length >= _minimum);
        }
    }
}