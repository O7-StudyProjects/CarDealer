using System.Diagnostics;
using System.Windows;
using Npgsql;
using Bogus;
using CarDealer.Models;

namespace CarDealer.Views;

public partial class MainWindowView : Window
{
	private NpgsqlDataSource DataSource { get; }

	public MainWindowView()
	{
		InitializeComponent();

		const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=debeast;Database=cardealer";
		DataSource = NpgsqlDataSource.Create(CONNECTION_STRING);
	}

	private async void GenerateMenuItem_OnClick(object sender, RoutedEventArgs e)
	{
		var watch = Stopwatch.StartNew();
		await Parallel.ForAsync(0, 100, async (_, token) =>
		{
			await using var data_source = DataSource;
			var faker = new Faker("ru");
			Car car = new()
			{
				Make = faker.Vehicle.Manufacturer(),
				Price = Convert.ToSingle(faker.Commerce.Price(1_000_000, 20_000_000)),
				Type = faker.Vehicle.Type().Replace(" ", null),
				Fuel = faker.Vehicle.Fuel()
			};
			Client client = new()
			{
				Name = faker.Person.FullName,
				Address = faker.Address.FullAddress(),
				PhoneNumber = faker.Phone.PhoneNumber("+7(###)###-####"),
				BankAccount = Convert.ToInt32(faker.Finance.Account()),
				Notes = faker.Lorem.Sentence()
			};
			Dealer dealer = new()
			{
				Name = faker.Company.CompanyName(),
				BankAccount = Convert.ToInt32(faker.Finance.Account()),
				Address = faker.Address.FullAddress(),
				CEO = faker.Person.FullName,
				ChiefAccountant = faker.Person.FullName
			};

			//todo sql parameters

			await using var car_command = data_source.CreateCommand(
				"INSERT INTO car (make, price, type, fuel) " +
				$"VALUES ({car.Make}, {car.Price}, {car.Type}, {car.Fuel}) RETURNING id"
			);
			await using var client_command = data_source.CreateCommand(
				"INSERT INTO client (name, address, phone_number, bank_account, notes) " +
				$"VALUES ({client.Name}, {client.Address}, {client.PhoneNumber}, {client.BankAccount}, {client.Notes}) RETURNING id"
			);
			await using var dealer_command = data_source.CreateCommand(
				"INSERT INTO dealer (name, bank_account, address, ceo, chief_accountant) " +
				$"VALUES ({dealer.Name}, {dealer.BankAccount}, {dealer.Address}, {dealer.CEO}, {dealer.ChiefAccountant}) RETURNING id"
			);

			car.Id = await car_command.ExecuteScalarAsync(token) as int? ?? throw new ("Car id is null");
			client.Id = await client_command.ExecuteScalarAsync(token) as int? ?? throw new ("Client id is null");
			dealer.Id = await dealer_command.ExecuteScalarAsync(token) as int? ?? throw new ("Dealer id is null");

			Contract contract = new()
			{
				Number = Convert.ToInt32(faker.Finance.Account(16)),
				Date = faker.Date.Past(),
				Amount = Convert.ToInt32(faker.Finance.Amount(1, 5)),
				CarId = car.Id,
				ClientId = client.Id,
				DealerId = dealer.Id
			};

			await using var contract_command = data_source.CreateCommand(
				"INSERT INTO contract (number, date, amount, client_id, car_id, dealer_id) " +
				$"VALUES ({contract.Number}, {contract.Date}, {contract.Amount}, {contract.ClientId}, {contract.CarId}, {contract.DealerId}) RETURNING id"
			);
			contract.Id = await contract_command.ExecuteScalarAsync(token) as int? ?? throw new ("Contract id is null");

			AccountStatement statement = new()
			{
				Number = Convert.ToInt32(faker.Finance.Account(16)),
				Date = faker.Date.Past(),
				Total = contract.Amount * car.Price,
				ContractId = contract.Id,
				IsPaid = faker.Random.Bool(),
			};
			if (statement.IsPaid)
			{
				statement.IsShipped = faker.Random.Bool();
			}

			await using var statement_command = data_source.CreateCommand(
				"INSERT INTO statement (number, date, total, contract_id, is_paid, is_shipped) " +
				$"VALUES ({statement.Number}, {statement.Date}, {statement.Total}, {statement.ContractId}, {statement.IsPaid}, {statement.IsShipped}) RETURNING id"
			);
			statement.Id = await statement_command.ExecuteScalarAsync(token) as int? ?? throw new ("Statement id is null");

			CarView.ViewModel.Cars.Add(car);
			ClientView.ViewModel.Clients.Add(client);
			DealerView.ViewModel.Dealers.Add(dealer);
			ContractView.ViewModel.Contracts.Add(contract);
			AccountStatementView.ViewModel.AccountStatements.Add(statement);
		});
		watch.Stop();
		LastOp.Header =	$"Last operation: {watch.ElapsedMilliseconds}ms";
	}

	private async void MenuItem_OnClick(object sender, RoutedEventArgs e)
	{
		await using var data_source = DataSource;
		await using var batch = data_source.CreateBatch();
		batch.BatchCommands.Add(new("DELETE FROM account_statement"));
		batch.BatchCommands.Add(new("DELETE FROM contract"));
		batch.BatchCommands.Add(new("DELETE FROM car"));
		batch.BatchCommands.Add(new("DELETE FROM client"));
		batch.BatchCommands.Add(new("DELETE FROM dealer"));
		await batch.ExecuteNonQueryAsync();
	}
}