namespace Brunozec.Common.Validators;

public sealed class FunctionResult<TEntity> : ValidationResult
{
    public TEntity Result { get; private set; }

    public FunctionResult()
        : this(default(TEntity))
    {
    }

    public FunctionResult(Func<TEntity> func)
        : this(func.Invoke())
    {

    }

    public FunctionResult(TEntity r)
        : this(r, new List<string>())
    {
    }

    public FunctionResult(params string[] errors)
        : this(errors.ToList())
    {
    }

    public FunctionResult(List<string> errors)
        : this(default(TEntity), errors)
    {
    }

    public FunctionResult(TEntity result, List<string> errors)
    {
        if (errors == null) throw new ArgumentNullException("errors");

        Result = result;
        Errors = errors;
    }

    public static implicit operator FunctionResult<TEntity>(TEntity entity) => new FunctionResult<TEntity>(entity);
}