namespace Brunozec.Common.Specifications.Assertions;

public class TrueAssertion : ISpecification<bool>
{
    public virtual Task<bool> IsSatisfiedBy(bool value)
    {
        return Task.FromResult(value);
    }
}