namespace Brunozec.Common.Specifications;

public class ExpressionSpecification<TEntity> : ISpecification<TEntity>
{
    public ExpressionSpecification(Func<TEntity, bool> expression)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    protected Func<TEntity, bool> Expression { get; }

    public Task<bool> IsSatisfiedBy(TEntity entity)
    {
        return Task.FromResult(Expression(entity));
    }
}