using System;
using System.Threading.Tasks;

namespace GameMenu.UI;

public class MainMenuItem {

	private readonly Func<Task> _executeAsync;

	public string Display { get; }

	public Task ExecuteAsync()
		=> _executeAsync();

	public MainMenuItem(string display, Func<Task> executeAsync) {
		Display = display;
		_executeAsync = executeAsync;
	}

}
