namespace Brunozec.Common.Validators;

/// <summary>
/// Contains the result of a <see cref="IValidator{TEntity}.Validate" /> method call.
/// </summary>
[Serializable]
public class ValidationResult
{
    /// <summary>
    /// Gets wheater the validation operation on an entity was valid or not.
    /// </summary>
    public bool IsValid
    {
        get { return Errors.Count == 0; }
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> that can be used to enumerate over
    /// the validation errors as a result of a <see cref="IValidator{TEntity}.Validate" /> method
    /// call.
    /// </summary>
    public List<string> Errors { get; protected set; }

    public ValidationResult()
    {
        Errors = new List<string>();
    }

    /// <summary>
    /// Adds a validation error into the result.
    /// </summary>
    /// <param name="error"></param>
    public ValidationResult AddError(string error)
    {
        Errors.Add(error);

        return this;
    }

    /// <summary>
    /// Adds many validation errors into the result.
    /// </summary>
    /// <param name="errors">The collection of errors</param>
    public ValidationResult AddErrors(IEnumerable<string> errors)
    {
        Errors = Errors.Union(errors).ToList();

        return this;
    }

    /// <summary>
    /// Removes a validation error from the result.
    /// </summary>
    /// <param name="error">The error content</param>
    public ValidationResult RemoveError(string error)
    {
        if (Errors.Contains(error))
            Errors.Remove(error);

        return this;
    }

    public string InvokeAfter { get; set; }

    public ValidationResult SetInvokeAfter(string url)
    {
        InvokeAfter = url;

        return this;
    }
}