using System.Collections.ObjectModel;
using System.Windows.Input;
using CarDealer.Core;
using CarDealer.Models;

namespace CarDealer.ViewModels;

public class CarViewModel : ViewModelBase
{
	public ObservableCollection<Car> Cars { get; } = [];
}