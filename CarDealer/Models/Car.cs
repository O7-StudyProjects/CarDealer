namespace CarDealer.Models;

public record Car
{
	public int Id { get; set; }
	public required string Make { get; set; }
	public required float Price { get; set; }
	public string? Type { get; set; }
	public string? Fuel { get; set; }
}
