using System.Windows.Input;
using Bogus;
using CarDealer.Core;
using CarDealer.Models;
using Npgsql;

namespace CarDealer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	private static readonly Faker FAKER = new ("ru");
	private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=debeast;Database=cardealer;Include Error Detail=True";
	public const int RECORDS_TO_GENERATE = 5000;

	private NpgsqlDataSource DataSource { get; } = NpgsqlDataSource.Create(CONNECTION_STRING);

	public async Task<(IList<Car>, IList<Client>, IList<Dealer>, IList<Contract>, IList<AccountStatement>)> GenerateData(bool is_parallel = true)
	{
		List<Car> cars = [];
		List<Client> clients = [];
		List<Dealer> dealers = [];
		List<Contract> contracts = [];
		List<AccountStatement> statements = [];
		if (is_parallel)
		{
			object lock_ = new();
			await Parallel.ForAsync(0, RECORDS_TO_GENERATE, async (_, _) =>
			{
				Car car = await GenerateCar();
				Client client = await GenerateClient();
				Dealer dealer = await GenerateDealer();
				Contract contract = await GenerateContract(car.Id, client.Id, dealer.Id);
				AccountStatement statement = await GenerateAccountStatement(contract, car);
				lock (lock_)
				{
					cars.Add(car);
					clients.Add(client);
					dealers.Add(dealer);
					contracts.Add(contract);
					statements.Add(statement);
				}
			});
		}
		else
		{
			for (int i = 0; i < RECORDS_TO_GENERATE; i++)
			{
				Car car = await GenerateCar();
				Client client = await GenerateClient();
				Dealer dealer = await GenerateDealer();
				Contract contract = await GenerateContract(car.Id, client.Id, dealer.Id);
				AccountStatement statement = await GenerateAccountStatement(contract, car);
				cars.Add(car);
				clients.Add(client);
				dealers.Add(dealer);
				contracts.Add(contract);
				statements.Add(statement);
			}
		}

		return (cars, clients, dealers, contracts, statements);
	}

	private async Task<AccountStatement> GenerateAccountStatement(Contract contract, Car car)
	{
		AccountStatement statement = new()
		{
			Number = Convert.ToInt64(FAKER.Finance.Account(16)),
			Date = FAKER.Date.Between(contract.Date, DateTime.Now),
			Total = contract.Amount * car.Price,
			ContractId = contract.Id,
			IsPaid = FAKER.Random.Bool(),
		};
		if (statement.IsPaid)
		{
			statement.IsShipped = FAKER.Random.Bool();
		}

		await using var statement_command = DataSource.CreateCommand(
			"INSERT INTO account_statement (number, date, total, contract_id, paid, shipped) " +
			"VALUES ($1, $2, $3, $4, $5, $6) RETURNING id"
		);
		statement_command.Parameters.Add(new(){Value = statement.Number});
		statement_command.Parameters.Add(new(){Value = statement.Date});
		statement_command.Parameters.Add(new(){Value = statement.Total});
		statement_command.Parameters.Add(new(){Value = statement.ContractId});
		statement_command.Parameters.Add(new(){Value = statement.IsPaid});
		statement_command.Parameters.Add(new(){Value = statement.IsShipped});
		statement.Id = await statement_command.ExecuteScalarAsync() as int? ?? throw new ("Statement id is null");
		return statement;
	}

	private async Task<Contract> GenerateContract(int car_id, int client_id, int dealer_id)
	{
		Contract contract = new()
		{
			Number = Convert.ToInt64(FAKER.Finance.Account(16)),
			Date = FAKER.Date.Past(),
			Amount = Convert.ToInt32(FAKER.Finance.Amount(1, 5)),
			CarId = car_id,
			ClientId = client_id,
			DealerId = dealer_id
		};
		await using var contract_command = DataSource.CreateCommand(
			"INSERT INTO contract (number, date, amount, client_id, car_id, dealer_id) " +
			"VALUES ($1, $2, $3, $4, $5, $6) RETURNING id"
		);
		contract_command.Parameters.Add(new(){Value = contract.Number});
		contract_command.Parameters.Add(new(){Value = contract.Date});
		contract_command.Parameters.Add(new(){Value = contract.Amount});
		contract_command.Parameters.Add(new(){Value = contract.ClientId});
		contract_command.Parameters.Add(new(){Value = contract.CarId});
		contract_command.Parameters.Add(new(){Value = contract.DealerId});
		contract.Id = await contract_command.ExecuteScalarAsync() as int? ?? throw new ("Contract id is null");

		return contract;
	}

	private async Task<Dealer> GenerateDealer()
	{
		Dealer dealer = new()
		{
			Name = FAKER.Company.CompanyName(),
			BankAccount = Convert.ToInt32(FAKER.Finance.Account()),
			Address = FAKER.Address.FullAddress(),
			CEO = FAKER.Person.FullName,
			ChiefAccountant = FAKER.Person.FullName
		};
		await using var dealer_command = DataSource.CreateCommand(
			"INSERT INTO dealer (name, bank_account, address, ceo, chief_accountant) " +
			"VALUES ($1, $2, $3, $4, $5) RETURNING id"
		);
		dealer_command.Parameters.Add(new(){Value = dealer.Name});
		dealer_command.Parameters.Add(new(){Value = dealer.BankAccount});
		dealer_command.Parameters.Add(new(){Value = dealer.Address});
		dealer_command.Parameters.Add(new(){Value = dealer.CEO});
		dealer_command.Parameters.Add(new(){Value = dealer.ChiefAccountant});
		dealer.Id = await dealer_command.ExecuteScalarAsync() as int? ?? throw new ("Dealer id is null");
		return dealer;
	}

	private async Task<Client> GenerateClient()
	{
		Client client = new()
		{
			Name = FAKER.Person.FullName,
			Address = FAKER.Address.FullAddress(),
			PhoneNumber = FAKER.Phone.PhoneNumber("+7(###)###-####"),
			BankAccount = Convert.ToInt32(FAKER.Finance.Account()),
			Notes = FAKER.Lorem.Sentence()
		};
		await using var client_command = DataSource.CreateCommand(
			"INSERT INTO client (name, address, phone_number, bank_account, notes) " +
			"VALUES ($1, $2, $3, $4, $5) RETURNING id"
		);
		client_command.Parameters.Add(new(){Value = client.Name});
		client_command.Parameters.Add(new(){Value = client.Address});
		client_command.Parameters.Add(new(){Value = client.PhoneNumber});
		client_command.Parameters.Add(new(){Value = client.BankAccount});
		client_command.Parameters.Add(new(){Value = client.Notes});
		client.Id = await client_command.ExecuteScalarAsync() as int? ?? throw new ("GenerateClient id is null");
		return client;
	}

	private async Task<Car> GenerateCar()
	{
		Car car = new()
		{
			Make = FAKER.Vehicle.Manufacturer(),
			Price = Convert.ToSingle(FAKER.Commerce.Price(1_000_000, 20_000_000)),
			Type = FAKER.Vehicle.Type(),
			Fuel = FAKER.Vehicle.Fuel()
		};
		await using var car_command = DataSource.CreateCommand(
			"INSERT INTO car (make, price, type, fuel) " +
			"VALUES ($1, $2, $3, $4) RETURNING id"
		);
		car_command.Parameters.Add(new(){Value = car.Make});
		car_command.Parameters.Add(new(){Value = car.Price});
		car_command.Parameters.Add(new(){Value = car.Type});
		car_command.Parameters.Add(new(){Value = car.Fuel});
		car.Id = await car_command.ExecuteScalarAsync() as int? ?? throw new ("Car id is null");

		return car;
	}

	public async Task ClearData()
	{
		await using var command = DataSource.CreateCommand(
			"TRUNCATE account_statement, contract, car, client, dealer RESTART IDENTITY"
		);
		await command.ExecuteNonQueryAsync();
	}
}