using System;
using System.Collections.Generic;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public interface IGroupParser
	{
		Tuple<string, string, string> GetOwnerInfo(string pageUrl);
		IEnumerable<Group> UpdateGroupList(IList<Group> currentGroups, string language);
	}
}