using WpfGroupFinder.ViewModels;

namespace WpfGroupFinder.Views
{
	/// <summary>
	/// Interaction logic for FireteamDetail.xaml
	/// </summary>
	public partial class FireteamDetailView
	{
		public FireteamDetailView()
		{
			InitializeComponent();
		}

		public void SetDataContext(FireteamDetailViewModel viewModel)
		{
			DataContext = viewModel;
		}
	}
}