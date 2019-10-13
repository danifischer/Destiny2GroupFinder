using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WpfGroupFinder.Models;

namespace WpfGroupFinder.Logic
{
	public class ReportParser
	{
		private IEnumerable<Activity> _activities;

		public ReportParser(IFileHandler fileHandler)
		{
			_activities = fileHandler.LoadHashInformation().Where(i => i.Type == "Raid");
		}

		public string GetClears(string bungieId, string raidType)
		{
			// Report AWS page
			// https://b9bv2wd97h.execute-api.us-west-2.amazonaws.com/prod/api/player/4611686018472629478
			// var pageUrl = $@"https://raid.report/pc/{bungieId}";
			var pageUrl = $@"https://b9bv2wd97h.execute-api.us-west-2.amazonaws.com/prod/api/player/{bungieId}";

			var doc = new HtmlAgilityPack.HtmlDocument();
			HtmlAgilityPack.HtmlNode.ElementsFlags["br"] = HtmlAgilityPack.HtmlElementFlag.Empty;
			doc.OptionWriteEmptyNodes = true;

			var raids = _activities.Where(i => i.ActivityName == raidType);

			try
			{
				var webRequest = HttpWebRequest.Create(pageUrl);
				var stream = webRequest.GetResponse().GetResponseStream();
				doc.Load(stream);
				stream.Close();

				var json = JObject.Parse(doc.DocumentNode.InnerHtml);
				var totalClears = json["response"]["clearsRank"]["value"].ToString();

				var activities = JArray.Parse(json["response"]["activities"].ToString());
				var clears = 0;

				foreach (var item in activities)
				{
					if (raids.Select(i => i.ActivityHash).Contains(item["activityHash"].ToString()))
					{
						clears += int.Parse(item["values"]["clears"].ToString());
					}
				}

				return clears + " (" + totalClears + ")";
			}
			catch
			{
				return "n/a";
			}	
		}
	}
}
