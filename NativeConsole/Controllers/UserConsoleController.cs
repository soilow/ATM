using Services;

namespace NativeConsole;

public class UserConsoleController : IUserConsoleController
{
    private readonly IAccountService _accountService;

    public UserConsoleController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<int> Run(CancellationToken cancellationToken)
    {
        Console.WriteLine("Введите свой ID:");
        if (!int.TryParse(Console.ReadLine(), out int accountId))
        {
            Console.WriteLine("Некорректный ввод ID. Попробуйте снова.");
            return 0;
        }

        Console.WriteLine("Введите свой ПИН-код:");
        string? inputPin = Console.ReadLine();
        if (string.IsNullOrEmpty(inputPin))
        {
            Console.WriteLine("PIN-код не может быть пустым.");
            return 0;
        }

        if (!int.TryParse(inputPin, out int pin))
        {
            Console.WriteLine("Некорректный формат PIN-Кода");
            return 0;
        }

        Entities.Account? account = await _accountService.GetAccountByIdAsync(accountId, cancellationToken).ConfigureAwait(false);
        if (account == null)
        {
            Console.WriteLine("Аккаунт не найден.");
            return 0;
        }

        if (account.Pin != pin)
        {
            Console.WriteLine("Неверный PIN-код.");
            return 0;
        }

        while (true)
        {
            Console.WriteLine("Выберите операцию:");
            Console.WriteLine("1 - Просмотреть баланс");
            Console.WriteLine("2 - Пополнить баланс");
            Console.WriteLine("3 - Снять деньги");
            Console.WriteLine("4 - Посмотреть ваши транзакции");
            Console.WriteLine("0 - Выход");

            string? operation = Console.ReadLine();
            Console.Clear();
            switch (operation)
            {
                case "1":
                    decimal balance = _accountService.GetBalance(cancellationToken);

                    Console.Clear();
                    Console.WriteLine($"Ваш баланс: {balance}");
                    Console.WriteLine();

                    break;

                case "2":
                    Console.WriteLine("Насколько хотите пополнить?");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        Console.WriteLine("Некорректная сумма!");
                    }

                    await _accountService.DepositAsync(amount, cancellationToken).ConfigureAwait(false);

                    break;

                case "3":
                    Console.WriteLine("Сколько хотите снять?");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal withdraw))
                    {
                        Console.WriteLine("Некорректная сумма!");
                    }

                    await _accountService.WithdrawAsync(withdraw, cancellationToken).ConfigureAwait(false);

                    break;

                case "4":
                    IList<Entities.Transaction> transactions = _accountService.GetTransactions(cancellationToken);

                    Console.Clear();

                    foreach (Entities.Transaction transaction in transactions)
                    {
                        if (transaction.Amount < 0)
                        {
                            Console.WriteLine($"Перевод с аккаунта {transaction.Accountid} на сумму {transaction.Amount}. Дата: {transaction.Date}");
                        }
                        else
                        {
                            Console.WriteLine($"Пополнение аккаунта {transaction.Accountid} на сумму {transaction.Amount}. Дата: {transaction.Date}");
                        }
                    }

                    break;

                case "0":
                    Console.WriteLine("Выход...");

                    return 0;

                default:
                    Console.WriteLine("Некорректный ввод. Повторите попытку");
                    break;
            }
        }
    }
}