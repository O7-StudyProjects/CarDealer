using Npgsql;

namespace CarDealer.Models;

public record Dealer
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required int BankAccount { get; set; }
	public required string Address { get; set; }
	public required string CEO { get; set; }
	public required string ChiefAccountant { get; set; }
}