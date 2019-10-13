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
		private readonly IUpdateTimer _updateTimer;
		private int _updateTime;
		private readonly IRaidCategorizer _raidCategorizer;

		public MainViewModel(IMessageBus messageBus, IGroupParser groupParser, 
			IFileHandler fileHandler, IEnumerable<Languages> languages, IUpdateTimer updateTimer, IRaidCategorizer raidCategorizer)
		{
			_messageBus = messageBus;
			_groupParser = groupParser;
			_fileHandler = fileHandler;
			_updateTimer = updateTimer;
			_raidCategorizer = raidCategorizer;

			Languages = new List<Languages>(languages);
			SelectedLanguage = Languages.FirstOrDefault();

			LoadGroupsFromFile();
			
			UpdateCommand = new RelayCommand(_ => UpdateGroups(), _ => !IsUpdatingGroups);
			UpdateGroups();

			UpdateTime = 0;
			_updateTimer.TimeoutAction = () => UpdateUpdateTime();
			_updateTimer.StartTimer(1000);
		}

		~MainViewModel()
		{
			_updateTimer.StopTimer();
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

		public int UpdateTime
		{
			get { return _updateTime; }
			set { _updateTime = this.RaiseAndSetIfChanged(ref _updateTime, value); }
		}

		private void UpdateUpdateTime()
		{
			UpdateTime++;
			if(UpdateTime > 60)
			{
				UpdateGroups();
			}
		}

		public ICommand UpdateCommand { get; }

		public ICollection<Languages> Languages { get; }

		public Languages SelectedLanguage { get; set; }

		private async void UpdateGroups()
		{
			try
			{
				IsUpdatingGroups = true;
				UpdateTime = 0;
				await Task.Run(() =>
				{
					LoadGroupsFromFile();
					var groups = _groupParser.UpdateGroupList(Groups.Select(i => i._model).ToList(), SelectedLanguage.LanguageShort)
										.Select(i => new GroupViewModel(i));
					Groups = new ObservableCollection<GroupViewModel>(groups.OrderBy(i => i.FirstSeenSort));
				}).ConfigureAwait(false);

				IsUpdatingGroups = false;
				var parser = new ReportParser(_fileHandler);

				await Task.Run(() =>
				{
					Parallel.ForEach(Groups.Where(k => k.Owner == ""), i =>
					{
						i.Type = _raidCategorizer.Categorize(i.Title);

						var info = _groupParser.GetOwnerInfo(i.Link);
						i.Owner = info.Item1;
						i.OwnerId = info.Item2;
						i.OwnerSteamId = info.Item3;

						i.Clears = parser.GetClears(i.OwnerId, i.Type.BungieName);
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