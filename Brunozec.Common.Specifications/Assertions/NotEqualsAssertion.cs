namespace Brunozec.Common.Specifications.Assertions;

public  class NotEqualsAssertion<T> : ISpecification<T>
{
    private readonly T _check;

    public NotEqualsAssertion(T check)
    {
        if (check == null)
        {
            throw new ArgumentNullException(nameof(check));
        }

        _check = check;
    }

    public virtual Task<bool> IsSatisfiedBy(T value)
    {
        return Task.FromResult(value == null || !_check.Equals(value));
    }
}

public sealed class NotEqualsAssertion : NotEqualsAssertion<object>
{
    public NotEqualsAssertion(object check)
        : base(check)
    {
    }
}