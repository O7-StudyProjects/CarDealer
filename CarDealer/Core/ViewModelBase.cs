using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarDealer.Core;

public class ViewModelBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? property_name = null)
	{
		PropertyChanged?.Invoke(this, new (property_name));
	}

	protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? property_name = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(property_name);
		return true;
	}
}