using Itmo.Dev.Platform.Postgres.Connection;
using Outcoming;

namespace Persistance;

public class PostgresTransactionRepository : ITransactionRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public PostgresTransactionRepository(IPostgresConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<List<Entities.Transaction>> GetTransactionsByAccountIdAsync(int accountId, CancellationToken cancellationToken)
    {
        Npgsql.NpgsqlConnection connection =
            await _connectionProvider.GetConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string query = "SELECT * FROM transactions WHERE account_id = @AccountId";
        Npgsql.NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@AccountID", accountId);

        var transactions = new List<Entities.Transaction>();
        Npgsql.NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            transactions.Add(new Entities.Transaction(
                reader.GetInt32(reader.GetOrdinal("account_id")),
                reader.GetDecimal(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("timestamp"))));
        }

        return transactions;
    }

    public async Task AddTransactionAsync(Entities.Transaction transaction, CancellationToken cancellationToken)
    {
        Npgsql.NpgsqlConnection connection =
            await _connectionProvider.GetConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string query = "INSERT INTO transactions (account_id, amount, timestamp) VALUES (@AccountId, @Amount, @Timestamp)";
        Npgsql.NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@AccountId", transaction.Accountid);
        command.Parameters.AddWithValue("@Amount", transaction.Amount);
        command.Parameters.AddWithValue("@Timestamp", transaction.Date);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}