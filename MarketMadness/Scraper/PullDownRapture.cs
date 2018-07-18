using System;
using System.ComponentModel;
using System.Linq;
using MarketMadness.Agent;
using MarketMadness.DataLayer;

namespace MarketMadness.Scraper
{
	public class PullDownRapture
	{
		private static ModelMadness DB = new ModelMadness();

		public static void Historical()
		{
			AgentAction action = new AgentAction("http://web.archive.org/web/*/http://raptureready.com/rap2.html", false);
			AgentDocument document = AgentHandler.Instance.PerformAction(new AgentSession(), action);
			string indexPage = document.ResponseString;
			int startIdx = indexPage.IndexOf("Feb 13, 2004");
			startIdx = indexPage.IndexOf("<a href=\"", startIdx) + 9;
			int stopIdx = indexPage.IndexOf("TEXT FOLLOWING URL");

			while (startIdx < stopIdx)
			{
				int endIdx = indexPage.IndexOf("\"", startIdx);
				string dateURL = indexPage.Substring(startIdx, endIdx - startIdx);

				SavePage(dateURL);

				startIdx = indexPage.IndexOf("<a href=\"", startIdx) + 9;
			}
		}

		private static void SavePage(string raptureURL)
		{
			AgentAction action = new AgentAction(raptureURL, false);

			AgentDocument document = null;
			int errorCount = 0;
			
			while (errorCount < 5)
				try
				{
					document = AgentHandler.Instance.PerformAction(new AgentSession(), action);
					break;
				}
				catch (Exception ex)
				{
					errorCount++;
				}

			int startIdx = document.ResponseString.IndexOf("Rapture Index ") + "Rapture Index ".Length;
			int endIdx = document.ResponseString.IndexOf("<", startIdx);
			int raptureIdx = int.Parse(document.ResponseString.Substring(startIdx, endIdx - startIdx));

			startIdx = document.ResponseString.IndexOf("Updated ") + "Updated ".Length;
			endIdx = document.ResponseString.IndexOf("<", startIdx);
			string dateString = document.ResponseString.Substring(startIdx, endIdx - startIdx);
			dateString = dateString.Replace("Sept", "Sep");

			DateTime raptureDate = (DateTime) new DateTimeConverter().ConvertFrom(dateString);

			if (!DB.RaptureIndexes.Where(x => x.rapture_day.Equals(raptureDate)).Any())
			{
				int oil = GetRaptureAmount(document.ResponseString, "Oil Supply", dateString);
				int debt = GetRaptureAmount(document.ResponseString, "Debt and Trade", dateString);
				int weather = GetRaptureAmount(document.ResponseString, "Wild Weather", dateString);
				int globalism = GetRaptureAmount(document.ResponseString, "Globalism", dateString);
				int economy = GetRaptureAmount(document.ResponseString, "Economy", dateString);

				Console.WriteLine("Rapture on " + raptureDate.ToString("d") + " was " + raptureIdx + " - oil: " + oil + ", debt: " + debt +
					", weather: " + weather + ", globalism: " + globalism + ", economy: " + economy);

				RaptureIndex rapture = new RaptureIndex
				                       	{
				                       		rapture_day = raptureDate,
				                       		rapture_index = raptureIdx,
											oil = oil,
											debt = debt,
											weather = weather,
											globalism = globalism,
											economy = economy
				                       	};
				DB.AddToRaptureIndexes(rapture);
				DB.SaveChanges();
			}
			else
				Console.WriteLine("Rapture already happened on " + raptureDate.ToString("d"));
		}

		private static int GetRaptureAmount(string doc, string label, string date)
		{
			int startIdx = doc.IndexOf(label);
			startIdx = doc.IndexOf(label, startIdx + 1);

			if (startIdx < 0)
				throw new Exception("can't find " + label + " on " + date);

			startIdx = doc.IndexOf(">", startIdx) + 1;
			int endIdx = doc.IndexOf("<", startIdx);
			
			string amount = doc.Substring(startIdx, endIdx - startIdx)
				.Replace("\t", "").Replace(" ", "").Replace("\r", "").Replace("\n", "");

			return int.Parse(amount.Substring(0, 1));
		}
	}
}
