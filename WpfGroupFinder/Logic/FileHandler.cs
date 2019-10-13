using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class FileHandler : IFileHandler
	{
		private const string Filename = "groups";

		public IEnumerable<Group> LoadGroups(string languageShort)
		{
			var groups = new List<Group>();

			if (!File.Exists(GetLangaugeSpecificFilename(languageShort)))
			{
				return groups;
			}
			using (StreamReader file = File.OpenText(GetLangaugeSpecificFilename(languageShort)))
			{
				JsonSerializer serializer = new JsonSerializer();
				groups = (List<Group>)serializer.Deserialize(file, typeof(List<Group>));
			}

			return groups;
		}

		public IEnumerable<Activity> LoadHashInformation()
		{
			List<Activity> hashList = new List<Activity>();

			if (!File.Exists(@"hashList.json"))
			{
				return new ObservableCollection<Activity>(hashList);
			}
			using (StreamReader file = File.OpenText(@"hashList.json"))
			{
				JsonSerializer serializer = new JsonSerializer();
				hashList = (List<Activity>)serializer.Deserialize(file, typeof(List<Activity>));
			}

			return hashList;
		}

		public async Task SaveGroups(IEnumerable<Group> groups, string languageShort)
		{
			await Task.Run(() =>
			{
				using (StreamWriter file = File.CreateText(GetLangaugeSpecificFilename(languageShort)))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(file, groups);
				}
			});
		}

		public async Task SaveRaids(IEnumerable<RaidType> raidTypes)
		{
			await Task.Run(() =>
			{
				using (StreamWriter file = File.CreateText(@"raidTypes.json"))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(file, raidTypes);
				}
			});
		}

		public IEnumerable<RaidType> LoadRaids()
		{
			List<RaidType> hashList = new List<RaidType>();

			if (!File.Exists(@"raidTypes.json"))
			{
				return Enumerable.Empty<RaidType>();
			}
			using (StreamReader file = File.OpenText(@"raidTypes.json"))
			{
				JsonSerializer serializer = new JsonSerializer();
				hashList = (List<RaidType>)serializer.Deserialize(file, typeof(List<RaidType>));
			}

			return hashList;
		}

		private string GetLangaugeSpecificFilename(string languageShort)
		{
			return Filename + "_" + languageShort + ".json";
		}
	}
}