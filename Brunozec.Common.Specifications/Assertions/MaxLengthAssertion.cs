namespace Brunozec.Common.Specifications.Assertions;

public class MaxLengthAssertion : ISpecification<string>
{
    private readonly int _maximum;

    public MaxLengthAssertion(int maximum)
    {
        _maximum = maximum;
    }

    public virtual Task<bool> IsSatisfiedBy(string value)
    {
        return Task.FromResult(string.IsNullOrWhiteSpace(value) || value.Trim().Length <= _maximum);
    }
}