using Entities;

namespace Services;

public interface IAccountService : IUserAccountService, IAdminAccountService
{
    Task<Account?> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken);
}