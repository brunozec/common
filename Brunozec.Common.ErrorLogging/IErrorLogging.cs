namespace Brunozec.Common.ErrorLogging;

public interface IErrorLogging
{
    void Log(string error);

    void Log(Exception ex);
}