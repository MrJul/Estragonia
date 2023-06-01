using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GameMenu.UI;

public abstract class ViewModel : INotifyPropertyChanged {

	public event PropertyChangedEventHandler? PropertyChanged;

	private Task? _loadTask;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
		if (EqualityComparer<T>.Default.Equals(field, value))
			return false;

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	public Task EnsureLoadedAsync()
		=> _loadTask ??= LoadAsync();

	protected abstract Task LoadAsync();

	public virtual Task<bool> TryCloseAsync()
		=> Task.FromResult(true);

}
