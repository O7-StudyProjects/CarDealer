using System.Collections.ObjectModel;
using CarDealer.Core;
using CarDealer.Models;

namespace CarDealer.ViewModels;

public class DealerViewModel : ViewModelBase
{
	public ObservableCollection<Dealer> Dealers { get; } = [];
}