namespace Brunozec.Common.Specifications.Assertions;

public class EqualsAssertion<T> : ISpecification<T>
{
    private readonly T _check;

    public EqualsAssertion(T check)
    {
        if (check == null)
        {
            throw new ArgumentNullException(nameof(check));
        }

        _check = check;
    }

    public virtual Task<bool> IsSatisfiedBy(T value)
    {
        return Task.FromResult(value != null && _check.Equals(value));
    }
}

public sealed class EqualsAssertion : EqualsAssertion<object>
{
    public EqualsAssertion(object check)
        : base(check)
    {
    }
}