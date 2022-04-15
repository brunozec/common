using Brunozec.Common.Validators;

namespace Brunozec.Common.Validation;

public interface IValidator<TEntity>
{
    Task<ValidationResult> Validate(TEntity entity);
}