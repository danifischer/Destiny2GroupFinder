using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class FileHandler : IFileHandler
	{
		private const string Filename = "groups.json";

		public IEnumerable<Group> LoadGroups()
		{
			var groups = new List<Group>();

			if (!File.Exists(Filename))
			{
				return groups;
			}
			using (StreamReader file = File.OpenText(Filename))
			{
				JsonSerializer serializer = new JsonSerializer();
				groups = (List<Group>)serializer.Deserialize(file, typeof(List<Group>));
			}

			return groups;
		}

		public async Task SaveGroups(IEnumerable<Group> groups)
		{
			await Task.Run(() =>
			{
				using (StreamWriter file = File.CreateText(Filename))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(file, groups);
				}
			});
		}
	}
}