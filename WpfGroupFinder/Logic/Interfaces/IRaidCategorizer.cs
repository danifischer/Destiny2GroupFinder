using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public interface IRaidCategorizer
	{
		RaidType Categorize(string title);
	}
}