using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Logic;

namespace WpfGroupFinder.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private readonly IGroupParser _groupParser;
		private readonly IMessageBus _messageBus;
		private bool _isUpdatingGroups;
		private ObservableCollection<GroupViewModel> _groups;

		public MainViewModel(IMessageBus messageBus, IGroupParser groupParser)
		{
			_messageBus = messageBus;
			_groupParser = groupParser;

			Groups = new ObservableCollection<GroupViewModel>();
			UpdateCommand = new RelayCommand(_ => UpdateGroups());
			UpdateGroups();
		}

		public ObservableCollection<GroupViewModel> Groups
		{
			get { return _groups; }
			set { _groups = this.RaiseAndSetIfChanged(ref _groups, value); }
		}

		public bool IsUpdatingGroups
		{
			get { return _isUpdatingGroups; }
			private set { _isUpdatingGroups = this.RaiseAndSetIfChanged(ref _isUpdatingGroups, value); }
		}

		public ICommand UpdateCommand { get; }

		private async void UpdateGroups()
		{
			try
			{
				IsUpdatingGroups = true;
				await Task.Run(() =>
				{
					var groups = _groupParser.UpdateGroupList(Groups.Select(i => i._model).ToList(), "de")
										.Select(i => new GroupViewModel(i));
					Groups = new ObservableCollection<GroupViewModel>(groups);
				}).ConfigureAwait(false);

				Parallel.ForEach(Groups.Where(k => k.Owner == ""), i =>
				{
					var info = _groupParser.GetOwnerInfo(i.Link);
					i.Owner = info.Item1;
					i.OwnerId = info.Item2;
				});

				IsUpdatingGroups = false;
			}
			catch
			{
				IsUpdatingGroups = false;
			}
		}
	}
}