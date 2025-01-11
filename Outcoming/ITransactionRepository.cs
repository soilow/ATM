namespace Outcoming;

public interface ITransactionRepository
{
    Task<List<Entities.Transaction>> GetTransactionsByAccountIdAsync(int accountId, CancellationToken cancellationToken);

    Task AddTransactionAsync(Entities.Transaction transaction, CancellationToken cancellationToken);
}