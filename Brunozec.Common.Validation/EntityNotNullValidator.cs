using System.Collections.Generic;
using System.Threading.Tasks;
using Brunozec.Common.Extensions;
using Brunozec.Common.Specifications.Assertions;
using Brunozec.Common.Validators;

namespace Brunozec.Common.Validation
{
    public class EntityNotNullValidator<T> : IValidator<T>
    {
        private readonly List<IValidationRule<T>> _rules;

        public EntityNotNullValidator(string message)
        {
            _rules = new List<IValidationRule<T>>
            {
                new AssertionRule<T, object>(i => i,
                    new NotNullAssertion(),
                    message)
            };
        }

        public virtual Task<ValidationResult> Validate(T entity)
        {
            return _rules.ValidateAllAsync(entity);
        }
    }
}