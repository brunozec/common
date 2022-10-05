namespace Brunozec.Common.Specifications.Assertions;

public class NotNullAssertion<T> : ISpecification<T>
{
    public virtual Task<bool> IsSatisfiedBy(T value)
    {
        return Task.FromResult(value != null);
    }
}

public sealed class NotNullAssertion : NotNullAssertion<object>
{
}