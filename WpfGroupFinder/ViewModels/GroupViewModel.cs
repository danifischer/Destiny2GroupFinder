using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.ViewModels
{
	public class GroupViewModel : ReactiveObject
	{
		internal Group _model;
		private readonly FireteamViewModelFactory _fireteamViewModelFactory;

		public GroupViewModel(Group group, FireteamViewModelFactory fireteamViewModelFactory)
		{
			_model = group;
			_fireteamViewModelFactory = fireteamViewModelFactory;
			OpenBungieCommand = new RelayCommand(_ => OpenGroup());
			OpenRaidReportCommand = new RelayCommand(_ => OpenReport());
			ShowDetailCommand = new RelayCommand(_ => ShowDetail());
		}

		private void ShowDetail()
		{
			_fireteamViewModelFactory.Create(this).Show();
		}

		public string FirstSeen => GetFirstSeenTime();
		public int FirstSeenSort => (int)(DateTime.Now - _model.FirstSeen).TotalMinutes;
		public string Link => _model.Link;
		public RelayCommand OpenBungieCommand { get; }
		public RelayCommand OpenRaidReportCommand { get; }
		public RelayCommand ShowDetailCommand { get; }

		public Guardian Owner
		{
			get { return _model.Fireteam?.SingleOrDefault(i => i.IsLeader); }
		}

		public IEnumerable<Guardian> Fireteam
		{
			get { return _model.Fireteam; }
			internal set
			{
				_model.Fireteam = value;
				this.RaisePropertyChanged(nameof(Owner));
			}
		}

		public string Space
		{
			get { return _model.Space; }
			internal set
			{
				_model.Space = value;
				this.RaisePropertyChanged();
			}
		}

		public string Time
		{
			get { return _model.Time; }
			internal set
			{
				_model.Time = value;
				this.RaisePropertyChanged();
			}
		}

		public RaidType Type
		{
			get { return _model.Type; }
			internal set
			{
				_model.Type = value;
				this.RaisePropertyChanged();
			}
		}

		public string Clears
		{
			get { return _model.Clears; }
			internal set
			{
				_model.Clears = value;
				this.RaisePropertyChanged();
			}
		}

		public bool Updated
		{
			get { return _model.Updated; }
			internal set
			{
				_model.Updated = value;
				this.RaisePropertyChanged();
			}
		}

		public string Title => _model.Title;

		public void OpenGroup()
		{
			Process.Start(_model.Link);
		}

		private string GetFirstSeenTime()
		{
			var minutes = (int)(DateTime.Now - _model.FirstSeen).TotalMinutes;
			if (minutes < 1)
			{
				return "now";
			}
			if (minutes > 60)
			{
				return "> 1h";
			}

			return minutes.ToString() + "m";
		}

		private void OpenReport()
		{
			if (Owner?.Id == "") return;
			var user = WebUtility.UrlEncode(Owner.Id);
			Process.Start($"https://raid.report/pc/{user}");
		}
	}
}