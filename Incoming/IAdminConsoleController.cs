namespace NativeConsole;

public interface IAdminConsoleController
{
    Task Run(CancellationToken cancellationToken);
}