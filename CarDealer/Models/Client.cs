namespace CarDealer.Models;

public record Client
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required int BankAccount { get; set; }
	public required string PhoneNumber { get; set; }
	public required string Address { get; set; }
	public string? Notes { get; set; }
}