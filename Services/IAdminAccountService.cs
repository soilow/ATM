using Entities;

namespace Services;

public interface IAdminAccountService
{
    Task<Account> CreateAccount(string name, decimal initialBalance, short userPin, CancellationToken cancellationToken);
}