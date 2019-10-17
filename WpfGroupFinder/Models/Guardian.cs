namespace WpfGroupFinder.Models
{
	public class Guardian
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string SteamId { get; set; }
		public bool IsLeader { get; set; }
		public int? ClearsActivity { get; set; }
		public int? ClearsTotal { get; set; }
	}
}