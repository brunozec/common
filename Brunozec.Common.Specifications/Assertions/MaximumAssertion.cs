namespace Brunozec.Common.Specifications.Assertions;

public class MaximumAssertion<T> : ISpecification<T> where T : IComparable<T>
{
    private readonly T _maximum;

    public MaximumAssertion(T maximum)
    {
        _maximum = maximum;
    }

    public virtual Task<bool> IsSatisfiedBy(T value)
    {
        return Task.FromResult(_maximum == null || value.CompareTo(_maximum) <= 0);
    }
}