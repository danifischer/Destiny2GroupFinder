using ReactiveUI;
using System;
using System.Diagnostics;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.ViewModels
{
	public class GroupViewModel : ReactiveObject
	{
		internal Group _model;

		public GroupViewModel(Group group)
		{
			_model = group;
		}

		public DateTime FirstSeen => _model.FirstSeen;

		public string Link => _model.Link;

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

		public string Title => _model.Title;

		public void OpenGroup()
		{
			Process.Start(_model.Link);
		}
	}
}