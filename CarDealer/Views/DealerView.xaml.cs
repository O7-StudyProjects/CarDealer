using System.Windows.Controls;
using CarDealer.ViewModels;

namespace CarDealer.Views;

public partial class DealerView : UserControl
{
	public DealerViewModel ViewModel => (DealerViewModel) DataContext;

	public DealerView()
	{
		InitializeComponent();
	}
}