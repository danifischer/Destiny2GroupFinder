using System;
using WpfGroupFinder.ViewModels;
using WpfGroupFinder.Views;

namespace WpfGroupFinder.Helper
{
	public class FireteamViewModelFactory
	{
		private readonly Func<FireteamDetailView> viewCreationFunc;

		public FireteamViewModelFactory(Func<FireteamDetailView> viewCreationFunc)
		{
			this.viewCreationFunc = viewCreationFunc;
		}

		public FireteamDetailViewModel Create(GroupViewModel groupViewModel)
		{
			return new FireteamDetailViewModel(groupViewModel, viewCreationFunc());
		}
	}
}