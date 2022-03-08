using System.Threading.Tasks;

namespace Brunozec.Common.Validators
{
    public interface IValidationRule<TEntity>
    {
        /// <summary>
        /// Message for the rule is not satisfied
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Validates and indicates whether the validation rule is satisfied for the entity
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity" /> entity instance to validate</param>
        /// <returns>Indicates whether the rule is satisfied for the entity</returns>
        Task<bool> Validate(TEntity entity);
    }
}