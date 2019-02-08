using System.Windows;
using WpfGroupFinder.ViewModels;

namespace WpfGroupFinder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow(MainViewModel mainViewModel)
		{
			InitializeComponent();
			DataContext = mainViewModel;
		}
	}
}