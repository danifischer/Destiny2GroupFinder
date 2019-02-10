using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Logic;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private readonly IGroupParser _groupParser;
		private readonly IMessageBus _messageBus;
		private bool _isUpdatingGroups;
		private ObservableCollection<GroupViewModel> _groups;
		private readonly IFileHandler _fileHandler;

		public MainViewModel(IMessageBus messageBus, IGroupParser groupParser, IFileHandler fileHandler, IEnumerable<Languages> languages)
		{
			_messageBus = messageBus;
			_groupParser = groupParser;
			_fileHandler = fileHandler;

			Languages = new List<Languages>(languages);
			SelectedLanguage = Languages.FirstOrDefault();

			LoadGroupsFromFile();
			
			UpdateCommand = new RelayCommand(_ => UpdateGroups(), _ => !IsUpdatingGroups);
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

		public ICollection<Languages> Languages { get; }

		public Languages SelectedLanguage { get; set; }

		private async void UpdateGroups()
		{
			try
			{
				IsUpdatingGroups = true;
				await Task.Run(() =>
				{
					LoadGroupsFromFile();
					var groups = _groupParser.UpdateGroupList(Groups.Select(i => i._model).ToList(), SelectedLanguage.LanguageShort)
										.Select(i => new GroupViewModel(i));
					Groups = new ObservableCollection<GroupViewModel>(groups);
				}).ConfigureAwait(false);

				IsUpdatingGroups = false;

				await Task.Run(() =>
				{
					Parallel.ForEach(Groups.Where(k => k.Owner == ""), i =>
					{
						var info = _groupParser.GetOwnerInfo(i.Link);
						i.Owner = info.Item1;
						i.OwnerId = info.Item2;
					});
				});

				await _fileHandler.SaveGroups(Groups.Select(i => i._model), SelectedLanguage.LanguageShort);
			}
			catch
			{
				IsUpdatingGroups = false;
			}
		}

		private void LoadGroupsFromFile()
		{
			var groups = _fileHandler.LoadGroups(SelectedLanguage.LanguageShort).Select(i => new GroupViewModel(i));
			Groups = new ObservableCollection<GroupViewModel>(groups);
		}
	}
}