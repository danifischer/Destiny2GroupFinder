using System.Collections.Generic;

namespace WpfGroupFinder.Models
{
	public class RaidType
	{
		public string ShortName { get; set; }
		public string BungieName { get; set; }
		public List<string> Tags { get; set; }
	}
}