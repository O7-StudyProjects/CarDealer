using System.Collections.ObjectModel;
using CarDealer.Core;
using CarDealer.Models;

namespace CarDealer.ViewModels;

public class AccountStatementViewModel : ViewModelBase
{
	public ObservableCollection<AccountStatement> AccountStatements { get; } = [];
}