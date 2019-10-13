using System.Collections.Generic;
using System.Linq;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class RaidCategorizer : IRaidCategorizer
	{
		private IEnumerable<RaidType> raidTypes = new List<RaidType>()
		{
			new RaidType() { BungieName = "Last Wish: Level 55", ShortName = "LW", Tags = new List<string>() { "last wish", "lw", "riven", "wunsch", "last", "lastwish" } },
			new RaidType() { BungieName = "Scourge of the Past", ShortName = "SOTP", Tags = new List<string>() { "sotp", "geißel", "scourge", "gdv", "geisel" } },
			new RaidType() { BungieName = "Leviathan", ShortName = "LEVI", Tags = new List<string>() { "levi", "leviathan", "callus", "calus", "acrius" } },
			new RaidType() { BungieName = "Leviathan, Eater of Worlds", ShortName = "EOW", Tags = new List<string>() { "argos", "weltenverschlinger", "eow", "eater" } },
			new RaidType() { BungieName = "Leviathan, Spire of Stars", ShortName = "SOS", Tags = new List<string>() { "sos", "spire" } },
			new RaidType() { BungieName = "Crown of Sorrow: Normal", ShortName = "COS", Tags = new List<string>() { "cos", "crown" } },
			new RaidType() { BungieName = "Garden of Salvation", ShortName = "GOS", Tags = new List<string>() { "gos", "garden" } },
		};

		public RaidCategorizer(IFileHandler fileHandler)
		{
			var raids = fileHandler.LoadRaids();
			if (raids.Any())
			{
				raidTypes = raids;
			}
			else
			{
				fileHandler.SaveRaids(raidTypes);
			}
		}

		public RaidType Categorize(string title)
		{
			foreach (var raidType in raidTypes)
			{
				foreach (var tag in raidType.Tags)
				{
					if (title.ToLower().Contains(tag)) return raidType;
				}
			}

			return new RaidType() { BungieName = "", ShortName = "n/a" };
		}
	}
}