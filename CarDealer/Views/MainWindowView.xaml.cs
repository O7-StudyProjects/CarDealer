using System.Diagnostics;
using System.Windows;
using Npgsql;
using Bogus;
using CarDealer.Models;
using CarDealer.ViewModels;

namespace CarDealer.Views;

public partial class MainWindowView : Window
{
	public MainWindowViewModel ViewModel => (MainWindowViewModel) DataContext;

	public MainWindowView()
	{
		InitializeComponent();
	}

	private void UpdateLastOpHeader_Generate(Stopwatch watch, bool is_parallel)
	{
		LastOp.Header =
			$"Last operation: {watch.ElapsedMilliseconds}ms to generate {MainWindowViewModel.RECORDS_TO_GENERATE} records {(is_parallel ? "in parallel" : "consecutively")}";
	}
	private void UpdateLastOpHeader_Clear(Stopwatch watch)
	{
		LastOp.Header =
			$"Last operation: {watch.ElapsedMilliseconds}ms to clear all records";
	}

	private async void GenerateMenuItem_OnClick(object sender, RoutedEventArgs e)
	{
		var watch = Stopwatch.StartNew();
		bool is_parallel = ReferenceEquals(sender, ParallelItem);
		var (cars, clients, dealers, contracts, account_statements) = await ViewModel.GenerateData(is_parallel);
		watch.Stop();
		foreach (Car car in cars)
		{
			CarView.ViewModel.Cars.Add(car);
		}

		foreach (Client client in clients)
		{
			ClientView.ViewModel.Clients.Add(client);
		}

		foreach (Dealer dealer in dealers)
		{
			DealerView.ViewModel.Dealers.Add(dealer);
		}

		foreach (Contract contract in contracts)
		{
			ContractView.ViewModel.Contracts.Add(contract);
		}

		foreach (AccountStatement account_statement in account_statements)
		{
			AccountStatementView.ViewModel.AccountStatements.Add(account_statement);
		}

		UpdateLastOpHeader_Generate(watch, is_parallel);
	}

	private async void ClearMenuItem_OnClick(object sender, RoutedEventArgs e)
	{
		var watch = Stopwatch.StartNew();
		await ViewModel.ClearData();
		CarView.ViewModel.Cars.Clear();
		ClientView.ViewModel.Clients.Clear();
		DealerView.ViewModel.Dealers.Clear();
		ContractView.ViewModel.Contracts.Clear();
		AccountStatementView.ViewModel.AccountStatements.Clear();
		watch.Stop();
		UpdateLastOpHeader_Clear(watch);
	}
}