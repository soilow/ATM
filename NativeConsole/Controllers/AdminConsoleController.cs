using Entities;
using Services;

namespace NativeConsole;

public class AdminConsoleController : IAdminConsoleController
{
    private readonly IAccountService _accountService;

    public AdminConsoleController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Task Run(CancellationToken cancellationToken)
    {
        Console.WriteLine("Введите админский пароль:");
        string? password = Console.ReadLine();

        if (password != "admin123")
        {
            Console.Clear();
            Console.WriteLine("Неправильный пароль. Доступ запрещен");
            Console.WriteLine();
            return Task.CompletedTask;
        }

        while (true)
        {
            Console.WriteLine("Административный мод. Выберите опции:");
            Console.WriteLine("1 - Добавить нового пользователя");
            Console.WriteLine("0 - Выход");

            string? operation = Console.ReadLine();
            Console.Clear();
            switch (operation)
            {
                case "1":
                    Console.WriteLine("Введите имя пользователя: ");
                    string? name = Console.ReadLine();

                    Console.WriteLine("Введите его изначальный баланс (по умолчанию 0)");
                    decimal? balance = null;

                    if (decimal.TryParse(Console.ReadLine(), out decimal decimalResult))
                    {
                        balance = decimalResult;
                    }

                    Console.WriteLine("Пусть пользователь введет ПИН-Код");
                    short? pin = null;

                    if (short.TryParse(Console.ReadLine(), out short pinResult))
                    {
                        pin = pinResult;
                    }

                    if (name is null || balance is null || pin is null)
                    {
                        return Task.CompletedTask;
                    }

                    decimal balenciage = balance.Value;
                    short shortik = pin.Value;

                    Task<Account> task = _accountService.CreateAccount(name, balenciage, shortik, cancellationToken);
                    int result = task.Result.Id;

                    Console.Clear();
                    Console.WriteLine($"ID нового пользователя: {result}");
                    Console.WriteLine("Пользователь должен использовать его для входа в систему");
                    Console.WriteLine();

                    break;

                case "0":
                    Console.Clear();
                    Console.WriteLine("Выход...");
                    Console.WriteLine();
                    return Task.CompletedTask;

                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте еще раз");
                    break;
            }
        }
    }
}