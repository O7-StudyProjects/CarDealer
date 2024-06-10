using System.Collections.ObjectModel;
using CarDealer.Core;
using CarDealer.Models;

namespace CarDealer.ViewModels;

public class ContractViewModel : ViewModelBase
{
	public ObservableCollection<Contract> Contracts { get; set; } = [];
}