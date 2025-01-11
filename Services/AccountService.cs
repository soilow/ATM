using Entities;
using Outcoming;

namespace Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private Account? _account = null;

    public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken)
    {
        _account = await _accountRepository.GetAccountByIdAsync(accountId, cancellationToken).ConfigureAwait(false);

        if (_account is null)
        {
            return null;
        }

        return _account;
    }

    public decimal GetBalance(CancellationToken cancellationToken)
    {
        if (_account is null)
        {
            return 0;
        }

        return _account.Balance;
    }

    public async Task<decimal> WithdrawAsync(decimal amount, CancellationToken cancellationToken)
    {
        if (_account is null)
        {
            return 0;
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Сумма пополнения должна быть больше нуля.");
        }

        if (_account.Balance < amount)
        {
            throw new ArgumentException("Недостаточно средств");
        }

        _account.Balance -= amount;
        await _accountRepository.UpdateAccountAsync(_account, cancellationToken).ConfigureAwait(false);

        var transaction = new Entities.Transaction(
            _account.Id,
            -amount,
            DateTime.UtcNow);

        await _transactionRepository.AddTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);

        return _account.Balance - amount;
    }

    public async Task<decimal> DepositAsync(decimal amount, CancellationToken cancellationToken)
    {
        if (_account is null)
        {
            return 0;
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Сумма пополнения должна быть больше нуля.");
        }

        _account.Balance += amount;
        await _accountRepository.UpdateAccountAsync(_account, cancellationToken).ConfigureAwait(false);

        var transaction = new Entities.Transaction(
            _account.Id,
            amount,
            DateTime.UtcNow);

        await _transactionRepository.AddTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);

        return _account.Balance + amount;
    }

    public IList<Entities.Transaction> GetTransactions(CancellationToken cancellationToken)
    {
        if (_account is null)
        {
            return new List<Entities.Transaction>();
        }

        Task<List<Entities.Transaction>> transactoins = _transactionRepository.GetTransactionsByAccountIdAsync(_account.Id, cancellationToken);
        List<Entities.Transaction> transactions = transactoins.Result;

        return transactions;
    }

    public async Task<Account> CreateAccount(string name, decimal initialBalance, short userPin, CancellationToken cancellationToken)
    {
        Account account = await _accountRepository.AddAccountAsync(name, initialBalance, userPin, cancellationToken).ConfigureAwait(false);

        return account;
    }
}