using System.Collections.Generic;
using System.Threading.Tasks;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public interface IFileHandler
	{
		IEnumerable<Group> LoadGroups(string languageShort);
		Task SaveGroups(IEnumerable<Group> groups, string languageShort);
	}
}