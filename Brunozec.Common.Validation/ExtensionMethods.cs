using Brunozec.Common.Validators;

namespace Brunozec.Common.Validation;

public static class ExtensionMethods
{
    /// <summary>
    /// Creates command result to perform validation of a validator. If validation is false, returns the result with
    /// the entity and errors. If validation is true, executes the action parameter and returns the result with
    /// the entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="validator">Validator to be validated that implements the interface IValidator</param>
    /// <param name="entity">Instance of entity</param>
    /// <param name="action">Action to be invoked if the validation is true</param>
    /// <returns>Command result with the entity and errors (if validation is false)</returns>
    public static async Task<FunctionResult<TEntity>> ResultAsync<TEntity>(this IValidator<TEntity> validator,
        TEntity entity, Func<Task> action = null)
    {
        var validation = await validator.Validate(entity);

        if (validation.IsValid)
        {
            if (action != null)
            {
                await action.Invoke();
            }

            return new FunctionResult<TEntity>(entity);
        }

        return new FunctionResult<TEntity>(entity, validation.Errors);
    }
        
    /// <summary>
    /// Creates command result to perform validation of a validator. If validation is false, returns the result with
    /// the entity and errors. If validation is true, executes the action parameter and returns the result with
    /// the entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="validator">Validator to be validated that implements the interface IValidator</param>
    /// <param name="entity">Instance of entity</param>
    /// <param name="action">Action to be invoked if the validation is true</param>
    /// <returns>Command result with the entity and errors (if validation is false)</returns>
    public static async Task<FunctionResult<TEntity>> ResultAsync<TEntity>(this IValidator<TEntity> validator,
        TEntity entity, Func<Task<FunctionResult<TEntity>>> action)
    {
        var validation = await validator.Validate(entity);

        if (validation.IsValid)
        {
            if (action != null)
            {
                return await action.Invoke();
            }

            return new FunctionResult<TEntity>(entity);
        }

        return new FunctionResult<TEntity>(entity, validation.Errors);
    }

}