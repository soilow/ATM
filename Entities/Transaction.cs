namespace Entities;

public class Transaction
{
    public int Accountid { get; init; }

    public decimal Amount { get; init; }

    public DateTime Date { get; init; }

    public Transaction(int accountid, decimal amount, DateTime date)
    {
        Accountid = accountid;
        Amount = amount;
        Date = date;
    }
}