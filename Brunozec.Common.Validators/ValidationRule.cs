using Brunozec.Common.Specifications;

namespace Brunozec.Common.Validators;

public class ValidationRuleAsync<TEntity> : IValidationRule<TEntity>
{
    /// <summary>
    /// Validates the rule to this specification is satisfied
    /// </summary>
    private readonly ISpecification<TEntity> _ifTrue;

    /// <summary>
    /// The underlying rule as a specification
    /// </summary>
    private readonly ISpecification<TEntity> _rule;

    /// <summary>
    /// The validation message associated with the rule.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Default Constructor.
    /// Creates a new instance of the <see cref="ValidationRule{TEntity}" /> class.
    /// </summary>
    /// <param name="rule">The underlying rule as a specification</param>
    /// <param name="message">The validation message associated with the rule.</param>
    /// <param name="ifTrue">(Optional) Validates the rule to this specification is satisfied</param>
    public ValidationRuleAsync(ISpecification<TEntity> rule, string message, ISpecification<TEntity> ifTrue = null)
    {
        _rule = rule;
        Message = message;
        _ifTrue = ifTrue;
    }

    /// <summary>
    /// Validates and indicates whether the validation rule is satisfied for the entity
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity" /> entity instance to validate</param>
    /// <returns>Indicates whether the rule is satisfied for the entity</returns>
    public virtual async Task<bool> Validate(TEntity entity)
    {
        // if expression was passed and is not satisfied, returns rule as true
        if (!await _ifTrue.IsSatisfiedBy(entity))
            return true;

        return await _rule.IsSatisfiedBy(entity);
    }
}