using System.Collections.Generic;
using System.Threading.Tasks;
using Brunozec.Common.Validators;

namespace Brunozec.Common.Extensions
{
    public static class Extensions
    {
        public static async Task<ValidationResult> ValidateAllAsync<TEntity>(this IEnumerable<IValidationRule<TEntity>> rules, TEntity entity)
        {
            var validation = new ValidationResult();

            foreach (var rule in rules)
            {
                if (!await rule.Validate(entity))
                {
                    validation.AddError(rule.Message);
                }
            }

            return validation;
        }
        
        public static async Task<FunctionResult<IEnumerable<T>>> ToFunctionResult<T>(this Task<IEnumerable<T>> entity)
        {
            return new FunctionResult<IEnumerable<T>>(await entity);
        }

        public static async Task<FunctionResult<T>> ToFunctionResult<T>(this Task<T> entity) where T : class
        {
            return new FunctionResult<T>(await entity);
        }
    }
}