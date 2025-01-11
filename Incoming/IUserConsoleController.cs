namespace NativeConsole;

public interface IUserConsoleController
{
    Task<int> Run(CancellationToken cancellationToken);
}