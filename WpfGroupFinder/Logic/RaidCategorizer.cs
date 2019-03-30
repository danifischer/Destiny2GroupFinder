using System.Collections.Generic;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class RaidCategorizer : IRaidCategorizer
	{
		private List<string> _lastWishTags = new List<string>() { "last wish", "lw", "riven", "wunsch", "last", "lastwish" };
		private List<string> _sotpTags = new List<string>() { "sotp", "geißel", "scourge", "gdv", "geisel" };
		private List<string> _leviathanTags = new List<string>() { "levi", "leviathan", "callus", "calus", "acrius" };
		private List<string> _agrosTags = new List<string>() { "argos", "weltenverschlinger", "eow", "eater" };
		private List<string> _sosTags = new List<string>() { "sos", "spire" };

		public RaidType Categorize(string title)
		{
			foreach (var tag in _lastWishTags)
			{
				if (title.ToLower().Contains(tag)) return new RaidType() { TypeLong = "Last Wish: Level 55", TypeShort = "LW" };
			}
			foreach (var tag in _sotpTags)
			{
				if (title.ToLower().Contains(tag)) return new RaidType() { TypeLong = "Scourge of the Past", TypeShort = "SOTP" };
			}
			foreach (var tag in _leviathanTags)
			{
				if (title.ToLower().Contains(tag)) return new RaidType() { TypeLong = "Leviathan", TypeShort = "LEVI" };
			}
			foreach (var tag in _agrosTags)
			{
				if (title.ToLower().Contains(tag)) return new RaidType() { TypeLong = "Leviathan, Eater of Worlds", TypeShort = "EOW" };
			}
			foreach (var tag in _sosTags)
			{
				if (title.ToLower().Contains(tag)) return new RaidType() { TypeLong = "Leviathan, Spire of Stars", TypeShort = "SOS" };
			}

			return new RaidType() { TypeLong = "", TypeShort = "n/a" };
		}
	}
}