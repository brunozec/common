using Brunozec.Common.Validators;

namespace Brunozec.Common.Auth;

public interface IAuthProvider
{
    Task<FunctionResult<IAccountInfo>> Authenticate(IAccountInfo accountInfo);

    Task<FunctionResult<IAccountInfo>> Authorize(string login, string jwtoken);
}