using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Brunozec.Common.Specifications.Assertions;

public class MatchesAssertion : ISpecification<string>
{
    private readonly RegexOptions _options;
    private readonly string _pattern;

    public MatchesAssertion(string pattern, RegexOptions options = RegexOptions.None)
    {
        _pattern = pattern;
        _options = options;
    }

    public virtual Task<bool> IsSatisfiedBy(string value)
    {
        return Task.FromResult(value != null && new Regex(_pattern, _options).IsMatch(value));
    }
}