using System;

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
		public string Owner { get; set; }
		public string OwnerId { get; set; }
	}
}