using ReactiveUI;

namespace WpfGroupFinder.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private readonly IMessageBus _messageBus;

		public MainViewModel(IMessageBus messageBus)
		{
			_messageBus = messageBus;
		}
	}
}