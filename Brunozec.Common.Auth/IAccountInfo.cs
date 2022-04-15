namespace Brunozec.Common.Auth;

public interface IAccountInfo
{
    string Login { get; set; }
    
    string? Jwtoken { get; set; }
    
    Guid SessionId { get; set; }
}