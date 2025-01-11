namespace NativeConsole;

public class ConsoleInterface
{
    private readonly IUserConsoleController _userController;
    private readonly IAdminConsoleController _adminController;

    public ConsoleInterface(IUserConsoleController userController, IAdminConsoleController adminController)
    {
        _userController = userController;
        _adminController = adminController;
    }

    public async Task Run(CancellationToken token)
    {
        while (true)
        {
            Console.WriteLine("Приветствуем в Хаилино-Банк");
            Console.WriteLine("Выберете опцию");
            Console.WriteLine("1 - Вход для клиентов");
            Console.WriteLine("2 - Вход для администраторов");
            Console.WriteLine("0 - Выход");

            string? input = Console.ReadLine();
            Console.Clear();
            switch (input)
            {
                case "1":
                    await _userController.Run(token).ConfigureAwait(false);
                    break;
                case "2":
                    await _adminController.Run(token).ConfigureAwait(false);
                    break;
                case "0":
                    Console.WriteLine("Выход...");
                    return;
                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте еще раз");
                    Console.WriteLine();
                    break;
            }
        }
    }
}