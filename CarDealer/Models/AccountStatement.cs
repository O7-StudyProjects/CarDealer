namespace CarDealer.Models;

public record AccountStatement
{
	public int Id { get; set; }
	public required long Number { get; set; }
	public required int ContractId { get; set; }
	public required DateTime Date { get; set; }
	public required float Total { get; set; }
	public bool IsPaid { get; set; }
	public bool IsShipped { get; set; }
}