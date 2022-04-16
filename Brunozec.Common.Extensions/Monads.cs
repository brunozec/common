using Brunozec.Common.Repository;
using Brunozec.Common.Validators;

namespace Brunozec.Common.Extensions;

public static class Monads
{
    public static async Task<FunctionResult<T>> Then<T>(this Task<FunctionResult<T>> validation, Func<T, Task<T>> func)
    {
        try
        {
            var validationResult = await validation;
            if (validationResult.IsValid)
            {
                await func.Invoke(validationResult.Result);
            }

            return validationResult;
        }
        catch (Exception ex)
        {
            return new FunctionResult<T>(ex.Message);
        }
    }

    public static async Task<FunctionResult<T>> Then<T>(this Task<FunctionResult<T>> validation, Func<T, Task> func)
    {
        try
        {
            var validationResult = await validation;
            if (validationResult.IsValid)
            {
                await func.Invoke(validationResult.Result);
            }

            return validationResult;
        }
        catch (Exception ex)
        {
            return new FunctionResult<T>(ex.Message);
        }
    }

    public static async Task<FunctionResult<T2>> Then<T, T2>(this Task<FunctionResult<T>> validation, Func<T, Task<FunctionResult<T2>>> func)
    {
        try
        {
            var validationResult = await validation;
            if (validationResult.IsValid)
            {
                return await func.Invoke(validationResult.Result);
            }

            return new FunctionResult<T2>(validationResult.Errors);
        }
        catch (Exception ex)
        {
            return new FunctionResult<T2>(ex.Message);
        }
    }

    public static ValidationResult IfValid(this ValidationResult validation, Action action)
    {
        if (validation.IsValid)
        {
            action.Invoke();
        }

        return validation;
    }

    public static ValidationResult IfValid<T>(this FunctionResult<T> functionResult, Func<T, ValidationResult> func)
    {
        if (functionResult.IsValid)
        {
            return func.Invoke(functionResult.Result);
        }

        return functionResult;
    }

    public static async Task<FunctionResult<T>> Commit<T>(this Task<FunctionResult<T>> funcValidation, IBaseContext uow)
    {
        try
        {
            var validation = await funcValidation;
            if (validation.IsValid)
            {
                await uow.CommitAsync();
            }

            return validation;
        }
        catch (Exception ex)
        {
            return new FunctionResult<T>(ex.Message);
        }
    }

    public static async Task<ValidationResult> Commit(this Task<ValidationResult> funcValidation, IBaseContext uow)
    {
        try
        {
            var validation = await funcValidation;
            if (validation.IsValid)
            {
                await uow.CommitAsync();
            }

            return validation;
        }
        catch (Exception ex)
        {
            return new ValidationResult(new List<string>
            {
                ex.Message
            });
        }
    }
}