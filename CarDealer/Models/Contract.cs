namespace CarDealer.Models;

public record Contract
{
	public int Id { get; set; }
	public required long Number { get; set; }
	public required DateTime Date { get; set; }
	public required int Amount { get; set; }
	public required int ClientId { get; set; }
	public required int CarId { get; set; }
	public required int DealerId { get; set; }
}