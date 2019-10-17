using ReactiveUI;
using System.Diagnostics;
using System.Net;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Views;

namespace WpfGroupFinder.ViewModels
{
	public class FireteamDetailViewModel : ReactiveObject
	{
		private readonly FireteamDetailView _view;

		public FireteamDetailViewModel(GroupViewModel groupViewModel, FireteamDetailView view)
		{
			GroupViewModel = groupViewModel;
			OpenRaidReportCommand = new RelayCommand(OpenReport);
			_view = view;
			_view.SetDataContext(this);
		}

		private void OpenReport(object userId)
		{
			if (userId.ToString() == "") return;
			var user = WebUtility.UrlEncode(userId.ToString());
			Process.Start($"https://raid.report/pc/{user}");
		}

		public GroupViewModel GroupViewModel { get; }

		public RelayCommand OpenRaidReportCommand { get; }

		public bool? Show()
		{
			this.RaisePropertyChanged(nameof(GroupViewModel));
			return _view.ShowDialog();
		}
	}
}