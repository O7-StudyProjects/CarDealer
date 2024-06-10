using System.Windows.Controls;
using CarDealer.ViewModels;

namespace CarDealer.Views;

public partial class ClientView : UserControl
{
	public ClientViewModel ViewModel => (ClientViewModel) DataContext;

	public ClientView()
	{
		InitializeComponent();
	}
}