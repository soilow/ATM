using Entities;

namespace Outcoming;

public interface IAccountRepository
{
    Task<Account> AddAccountAsync(string name, decimal initialBalance, short userPin, CancellationToken cancellationToken);

    Task<Account?> GetAccountByIdAsync(int id, CancellationToken cancellationToken);

    Task UpdateAccountAsync(Account account, CancellationToken cancellationToken);
}