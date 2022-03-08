using System.Threading.Tasks;

namespace Brunozec.Common.Specifications.Assertions
{
    public class LengthAssertion : ISpecification<string>
    {
        private readonly int _length;

        public LengthAssertion(int length)
        {
            _length = length;
        }

        public virtual Task<bool> IsSatisfiedBy(string value)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(value) && value.Trim().Length == _length);
        }
    }
}