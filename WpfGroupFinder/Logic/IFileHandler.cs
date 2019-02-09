using System.Collections.Generic;
using System.Threading.Tasks;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public interface IFileHandler
	{
		IEnumerable<Group> LoadGroups();
		Task SaveGroups(IEnumerable<Group> groups);
	}
}