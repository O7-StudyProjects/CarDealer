using System.Windows.Controls;
using System.Windows.Input;
using CarDealer.ViewModels;

namespace CarDealer.Views;

public partial class CarView : UserControl
{
	public CarViewModel ViewModel => (CarViewModel) DataContext;

	public CarView()
	{
		InitializeComponent();
	}
}