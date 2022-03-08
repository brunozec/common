using System;
using System.Threading.Tasks;
using Brunozec.Common.Specifications;

namespace Brunozec.Common.Validators
{
    /// <summary>
    /// Represents an assertion rule to an entity
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TProperty">Type of property to be validated by rule</typeparam>
    public class AssertionRule<TEntity, TProperty> : IValidationRule<TEntity>
    {
        /// <summary>
        /// Validates the rule to this specification is satisfied
        /// </summary>
        private readonly ISpecification<TProperty> _ifTrue;

        /// <summary>
        /// Expression that indicates the property to be validated by rule
        /// </summary>
        private readonly Func<TEntity, TProperty> _property;

        /// <summary>
        /// The underlying rule as a specification
        /// </summary>
        private readonly ISpecification<TProperty> _rule;

        /// <summary>
        /// The validation message associated with the rule.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="AssertionRule{TEntity, TProperty}" /> class
        /// </summary>
        /// <param name="property">Expression that indicates the property to be validated by rule</param>
        /// <param name="rule">The underlying rule as a specification</param>
        /// <param name="message">The validation message associated with the rule.</param>
        /// <param name="ifTrue">(Optional) Validates the rule to this specification is satisfied</param>
        public AssertionRule(Func<TEntity, TProperty> property, ISpecification<TProperty> rule, string message, ISpecification<TProperty> ifTrue = null)
        {
            _property = property;
            _rule = rule;

            Message = message;

            _ifTrue = ifTrue;
        }


        /// <summary>
        /// Validates and indicates whether the assertion rule is satisfied for the entity
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity" /> entity instance to validate</param>
        /// <returns>Indicates whether the rule is satisfied for the entity</returns>
        public virtual async Task<bool> Validate(TEntity entity)
        {
            var prop = _property(entity);

            // if expression was passed and is not satisfied, returns assertion as true
            if (_ifTrue != null && !await _ifTrue.IsSatisfiedBy(prop))
                return true;


            return await _rule.IsSatisfiedBy(prop);
        }


    }
}