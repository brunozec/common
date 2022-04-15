namespace Brunozec.Common.Specifications.Assertions;

public class MinimumAssertion<T> : ISpecification<T> where T : IComparable<T>
{
    private readonly T _minimum;

    public MinimumAssertion(T minimum)
    {
        _minimum = minimum;
    }

    public virtual Task<bool> IsSatisfiedBy(T value)
    {
        return Task.FromResult(_minimum != null && value.CompareTo(_minimum) >= 0);
    }
}