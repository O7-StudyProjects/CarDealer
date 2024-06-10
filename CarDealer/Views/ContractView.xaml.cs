using System.Windows.Controls;
using System.Windows.Input;
using CarDealer.ViewModels;

namespace CarDealer.Views;

public partial class ContractView : UserControl
{
	public ContractViewModel ViewModel => (ContractViewModel) DataContext;

	public ContractView()
	{
		InitializeComponent();
	}

	private void Cell_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		throw new NotImplementedException();
	}
}