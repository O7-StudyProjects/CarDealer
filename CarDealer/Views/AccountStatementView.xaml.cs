using System.Windows.Controls;
using System.Windows.Input;
using CarDealer.ViewModels;

namespace CarDealer.Views;

public partial class AccountStatementView : UserControl
{
	public AccountStatementViewModel ViewModel => (AccountStatementViewModel) DataContext;

	public AccountStatementView()
	{
		InitializeComponent();
	}

	private void Cell_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		throw new NotImplementedException();
	}
}