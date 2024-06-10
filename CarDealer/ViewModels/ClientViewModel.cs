using System.Collections.ObjectModel;
using CarDealer.Core;
using CarDealer.Models;

namespace CarDealer.ViewModels;

public class ClientViewModel : ViewModelBase
{
	public ObservableCollection<Client> Clients { get; set; } = [];
}