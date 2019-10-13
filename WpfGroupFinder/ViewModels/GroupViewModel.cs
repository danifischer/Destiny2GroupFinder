using ReactiveUI;
using System;
using System.Diagnostics;
using System.Net;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.ViewModels
{
	public class GroupViewModel : ReactiveObject
	{
		internal Group _model;

		public GroupViewModel(Group group)
		{
			_model = group;
			OpenBungieCommand = new RelayCommand(_ => OpenGroup());
			OpenRaidReportCommand = new RelayCommand(_ => OpenReport());
		}

		public string FirstSeen => GetFirstSeenTime();
		public int FirstSeenSort => (int)(DateTime.Now - _model.FirstSeen).TotalMinutes;
		public string Link => _model.Link;
		public RelayCommand OpenBungieCommand { get; }
		public RelayCommand OpenRaidReportCommand { get; }
		public string Owner
		{
			get { return _model.Owner; }
			internal set
			{
				_model.Owner = value;
				this.RaisePropertyChanged();
			}
		}

		public string OwnerId
		{
			get { return _model.OwnerId; }
			internal set
			{
				_model.OwnerId = value;
				this.RaisePropertyChanged();
			}
		}

		public string OwnerSteamId
		{
			get { return _model.OwnerSteamId; }
			internal set
			{
				_model.OwnerSteamId = value;
				this.RaisePropertyChanged();
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
			if (OwnerId == "") return;
			var user = WebUtility.UrlEncode(OwnerId);
			Process.Start($"https://raid.report/pc/{user}");
		}
	}
}