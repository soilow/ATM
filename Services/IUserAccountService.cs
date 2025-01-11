namespace Services;

public interface IUserAccountService
{
    decimal GetBalance(CancellationToken cancellationToken);

    Task<decimal> WithdrawAsync(decimal amount, CancellationToken cancellationToken);

    Task<decimal> DepositAsync(decimal amount, CancellationToken cancellationToken);

    IList<Entities.Transaction> GetTransactions(CancellationToken cancellationToken);
}