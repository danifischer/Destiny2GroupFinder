using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class GroupParser : IGroupParser
	{
		public IEnumerable<Guardian> GetFireteam(string pageUrl)
		{
			var doc = new HtmlAgilityPack.HtmlDocument();
			HtmlAgilityPack.HtmlNode.ElementsFlags["br"] = HtmlAgilityPack.HtmlElementFlag.Empty;
			doc.OptionWriteEmptyNodes = true;

			try
			{
				var webRequest = HttpWebRequest.Create(pageUrl);
				var stream = webRequest.GetResponse().GetResponseStream();
				doc.Load(stream);
				stream.Close();
			}
			catch
			{
				return null;
			}

			var items = doc.DocumentNode.SelectNodes("//li[contains(@class,'user-fireteam ')]");
			var list = new List<Guardian>();

			foreach (var user in items)
			{
				var isLeader = user.HasClass("leader");
				var membershipId = user.GetAttributeValue("data-membershipid", "");

				var nameNode = user.ChildNodes.Single(i => i.HasClass("user-data"))
					.ChildNodes.Single(j => j.HasClass("user-container"));

				var name = FormatFromWeb.Format(nameNode.ChildNodes.Single(j => j.HasClass("display-name")).InnerText);
				var steamId = FormatFromWeb.Format(nameNode.ChildNodes.Single(j => j.Name == "span").InnerText.Replace("ID: ", ""));

				list.Add(new Guardian() { Id = membershipId, Name = name, SteamId = steamId, IsLeader = isLeader });
			}

			return list;
		}

		public IEnumerable<Group> UpdateGroupList(IList<Group> currentGroups, string language)
		{
			var pageUrl = $@"https://www.bungie.net/en/ClanV2/FireteamSearch?platform=4&activityType=1&lang={language}&groupid=&";

			var doc = new HtmlAgilityPack.HtmlDocument();
			HtmlAgilityPack.HtmlNode.ElementsFlags["br"] = HtmlAgilityPack.HtmlElementFlag.Empty;
			doc.OptionWriteEmptyNodes = true;
			var list = new List<Models.Group>();

			try
			{
				var webRequest = HttpWebRequest.Create(pageUrl);
				var stream = webRequest.GetResponse().GetResponseStream();
				doc.Load(stream);
				stream.Close();
			}
			catch (System.UriFormatException)
			{
				throw;
			}
			catch (System.Net.WebException)
			{
				throw;
			}

			var items = doc.DocumentNode.SelectNodes("//a[contains(@class,'item-fireteam-card')]")?.ToArray();

			if (items == null)
			{
				return list;
			}

			for (int i = 0; i < items.Length; i++)
			{
				// Full groups should not be visible anymore
				//if (items[i].ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.HasClass("lockedTopic"))
				//{
				//	continue;
				//}

				var link = items[i].GetAttributeValue("href", "");
				var url = "https://www.bungie.net" + System.Web.HttpUtility.HtmlDecode(link);
				var item = items[i].SelectSingleNode(".//div[contains(@class,'fireteam-content')]")
					.SelectSingleNode(".//div[contains(@class,'fireteam-details')]");

				var title = FormatFromWeb.Format(item.SelectSingleNode(".//p[contains(@class,'title')]").InnerText);
				//var owner = GetOwnerInfo(url);
				var spaceTotal = item.SelectSingleNode(".//div[contains(@class,'player-slots')]").SelectNodes(".//div[contains(@class,'player-slot')]").Count();
				var spaceUsed = item.SelectSingleNode(".//div[contains(@class,'player-slots')]").SelectNodes(".//div[contains(@class,'player-slot used')]").Count();
				var space = spaceTotal - spaceUsed;
				var time = FormatFromWeb.Format(item.SelectSingleNode(".//div[contains(@class,'fireteam-meta')]")
					.SelectSingleNode(".//p[contains(@class,'meta creation-date')]").InnerText);
				if (space == 0)
				{
					continue;
				}
				var id = items[i].ParentNode.GetAttributeValue("data-id", "");

				if (space > 0)
				{
					list.Add(new Models.Group()
					{
						Id = id,
						FirstSeen = DateTime.Now,
						Title = title,
						Space = space.ToString(),
						Time = time,
						Link = url
					});
				}
			}

			var currentGroupsList = currentGroups.ToList();

			foreach (var item in list.Where(i => !currentGroupsList.Select(k => k.Id).Contains(i.Id)))
			{
				currentGroups.Add(item);
			}
			foreach (var item in currentGroupsList.Where(i => !list.Select(k => k.Id).Contains(i.Id)))
			{
				currentGroups.Remove(item);
			}

			foreach (var item in list.Where(i => currentGroupsList.Select(k => k.Id).Contains(i.Id)))
			{
				var group = currentGroups.Single(i => i.Id == item.Id);
				group.Space = item.Space;

				if (TimeSplicer(group.Time) > TimeSplicer(item.Time))
				{
					group.Updated = true;
				}

				group.Time = item.Time;
			}

			return currentGroups;
		}

		private static int TimeSplicer(string time)
		{
			if (time == "now") return 0;
			if (time.Contains("h")) return 99;

			return int.Parse(new string(time.Where(char.IsDigit).ToArray()));
		}
	}
}