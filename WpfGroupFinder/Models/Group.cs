using System;
using System.Collections.Generic;

namespace WpfGroupFinder.Models
{
	public class Group
	{
		public string Id { get; set; }
		public DateTime FirstSeen { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public string Space { get; set; }
		public string Time { get; set; }
		public IEnumerable<Guardian> Fireteam { get; set; }
		public RaidType Type { get; set; }
		public string Clears { get; set; }
		public bool Updated { get; set; }
	}
}