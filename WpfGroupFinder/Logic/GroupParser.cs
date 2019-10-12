using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using WpfGroupFinder.Helper;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class GroupParser : IGroupParser
	{
		public Tuple<string, string> GetOwnerInfo(string pageUrl)
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
				return new Tuple<string, string>("", null);
			}

			var item = doc.DocumentNode.SelectNodes("//li[contains(@class,'user-fireteam')]").First().ParentNode
				.SelectSingleNode("//li[contains(@class,'leader')]");
			var membershipId = item.GetAttributeValue("data-membershipid", "");

			var nameNode = item.ChildNodes.Single(i => i.HasClass("user-data"))
				.ChildNodes.Single(j => j.HasClass("user-container"))
				.ChildNodes.Single(j => j.HasClass("display-name"));
			var name = FormatFromWeb.Format(nameNode.InnerText);

			return new Tuple<string, string>(name, membershipId);
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
						Owner = "",//owner.Item1,
						OwnerId = "",//owner.Item2,
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
				group.Time = item.Time;
			}

			return currentGroups;
		}
	}
}