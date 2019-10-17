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
		private readonly IFileHandler _fileHandler;
		private readonly FireteamViewModelFactory _fireteamViewModelFactory;
		private readonly IGroupParser _groupParser;
		private readonly IMessageBus _messageBus;
		private readonly IRaidCategorizer _raidCategorizer;
		private readonly IUpdateTimer _updateTimer;
		private ObservableCollection<GroupViewModel> _groups;
		private bool _isUpdatingGroups;
		private int _updateTime;

		public MainViewModel(IMessageBus messageBus, IGroupParser groupParser,
			IFileHandler fileHandler, IEnumerable<Languages> languages, IUpdateTimer updateTimer,
			IRaidCategorizer raidCategorizer, FireteamViewModelFactory fireteamViewModelFactory)
		{
			_messageBus = messageBus;
			_groupParser = groupParser;
			_fileHandler = fileHandler;
			_updateTimer = updateTimer;
			_raidCategorizer = raidCategorizer;
			_fireteamViewModelFactory = fireteamViewModelFactory;

			Languages = new List<Languages>(languages);
			SelectedLanguage = Languages.FirstOrDefault();

			LoadGroupsFromFile();

			UpdateCommand = new RelayCommand(_ => UpdateGroups(), _ => !IsUpdatingGroups);
			OpenCommand = new RelayCommand(OpenDetail, _ => !IsUpdatingGroups);
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

		public ICollection<Languages> Languages { get; }

		public ICommand OpenCommand { get; }

		public Languages SelectedLanguage { get; set; }

		public ICommand UpdateCommand { get; }

		public int UpdateTime
		{
			get { return _updateTime; }
			set { _updateTime = this.RaiseAndSetIfChanged(ref _updateTime, value); }
		}

		private void LoadGroupsFromFile()
		{
			var groups = _fileHandler.LoadGroups(SelectedLanguage.LanguageShort).Select(i => new GroupViewModel(i, _fireteamViewModelFactory));
			Groups = new ObservableCollection<GroupViewModel>(groups);
		}

		private void OpenDetail(object obj)
		{
			var groupViewModel = obj as GroupViewModel;
			if (groupViewModel != null)
			{
				groupViewModel.ShowDetailCommand.Execute(null);
			}
		}

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
										.Select(i => new GroupViewModel(i, _fireteamViewModelFactory));
					Groups = new ObservableCollection<GroupViewModel>(groups.OrderBy(i => i.FirstSeenSort));
				}).ConfigureAwait(false);

				IsUpdatingGroups = false;
				var parser = new ReportParser(_fileHandler);

				await Task.Run(() =>
				{
					Parallel.ForEach(Groups.Where(k => k.Owner == null || k.Updated), i =>
					{
						i.Type = _raidCategorizer.Categorize(i.Title);
						i.Fireteam = _groupParser.GetFireteam(i.Link);
						foreach (var guardian in i.Fireteam)
						{
							parser.SetClears(guardian, i.Type.BungieName);
						}

						if (i.Fireteam.All(j => j.ClearsTotal == null))
						{
							i.Clears = "none";
						}
						else
						{
							i.Clears = string.Format("{0:0} ({1:0})", i.Fireteam.Average(j => j.ClearsActivity), i.Fireteam.Average(j => j.ClearsTotal));
						}
						i.Updated = false;
					});
				});

				await _fileHandler.SaveGroups(Groups.Select(i => i._model), SelectedLanguage.LanguageShort);
			}
			catch
			{
				IsUpdatingGroups = false;
			}
		}

		private void UpdateUpdateTime()
		{
			UpdateTime++;
			if (UpdateTime > 60)
			{
				UpdateGroups();
			}
		}
	}
}