namespace Entities;

public class Account
{
    public int Id { get; init; }

    public short Pin { get; init; }

    public string Name { get; init; }

    public decimal Balance { get; set; }

    public Account(int id, string name, decimal balance, short pin)
    {
        Id = id;
        Pin = pin;
        Name = name;
        Balance = balance;
    }
}