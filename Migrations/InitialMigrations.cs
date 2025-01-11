using FluentMigrator;
using Itmo.Dev.Platform.Postgres.Migrations;

namespace Migrations;

public class InitialMigrations
{
    [Migration(3, "Initial migration")]
    public class InitialMigration : SqlMigration
    {
        protected override string GetUpSql(IServiceProvider serviceProvider)
        {
            return """
                   CREATE TABLE accounts (
                       id SERIAL PRIMARY KEY,
                       pin SMALLINT,
                       name TEXT NOT NULL,
                       balance DECIMAL(18, 2) NOT NULL
                   );

                   CREATE TABLE transactions (
                       id SERIAL PRIMARY KEY,
                       account_id INT REFERENCES accounts(id) ON DELETE CASCADE NOT NULL,
                       amount DECIMAL(18, 2) NOT NULL,
                       timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                   );
                   """;
        }

        protected override string GetDownSql(IServiceProvider serviceProvider)
        {
            return """
                   DROP TABLE IF EXISTS accounts;
                   DROP TABLE IF EXISTS transactions;
                   """;
        }
    }
}