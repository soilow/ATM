using Entities;
using Itmo.Dev.Platform.Postgres.Connection;
using Outcoming;

namespace Persistance;

public class PostgresAccountRepository : IAccountRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public PostgresAccountRepository(IPostgresConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<Account> AddAccountAsync(string name, decimal initialBalance, short userPin, CancellationToken cancellationToken)
    {
        Npgsql.NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        const string query = "INSERT INTO accounts (name, balance, pin) VALUES (@Name, @Balance, @Pin) RETURNING id;";
        Npgsql.NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Balance", initialBalance);
        command.Parameters.AddWithValue("@Pin", userPin);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

        var account = new Account(Convert.ToInt32(result), name, initialBalance, userPin);

        return account;
    }

    public async Task<Account?> GetAccountByIdAsync(int id, CancellationToken cancellationToken)
    {
        Npgsql.NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        const string query = "SELECT * FROM accounts WHERE id = @Id";
        Npgsql.NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        Npgsql.NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return new Account(
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("name")),
                reader.GetDecimal(reader.GetOrdinal("balance")),
                reader.GetInt16(reader.GetOrdinal("pin")));
        }

        return null;
    }

    public async Task UpdateAccountAsync(Account account, CancellationToken cancellationToken)
    {
        Npgsql.NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        const string query = "Update accounts set balance = @Balance, pin = @Pin where id = @Id";
        Npgsql.NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Balance", account.Balance);
        command.Parameters.AddWithValue("@Pin", account.Pin);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
