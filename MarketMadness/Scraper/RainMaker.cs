using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketMadness.Agent;
using MarketMadness.DataLayer;

namespace MarketMadness.Scraper
{
	public class RainMaker
	{
		public static void Dance()
		{
			Console.WriteLine("Checking the weather");

			ModelMadness db = new ModelMadness();

			// from 2006-01-01 through present
			HashSet<DateTime> existingDays = new HashSet<DateTime>(db.Weathers.Select(x => x.day).ToArray());

			DateTime current = new DateTime(2006, 1, 1);
			
			while (current < DateTime.Now)
			{
				if (!existingDays.Contains(current))
					GetWeatherForDay(current, db);

				current = current.AddDays(1);
			}

			Console.WriteLine("Done");
		}

		private static void GetWeatherForDay(DateTime day, ModelMadness db)
		{
			string url =
				"http://www.wunderground.com/history/airport/KNYC/`/DailyHistory.html?req_city=New+York&req_state=NY&req_statename=New+York&MR=1";

			url = url.Replace("`", day.Year + "/" + day.Month + "/" + day.Day);
			AgentAction action = new AgentAction(url, false);
			AgentDocument document = AgentHandler.Instance.PerformAction(new AgentSession(), action);
			int startIdx = document.ResponseString.IndexOf("Mean Temperature");
			startIdx = document.ResponseString.IndexOf("<span class=\"b\">", startIdx) + "<span class=\"b\">".Length;
			int endidx = document.ResponseString.IndexOf("<", startIdx);
			int actual = int.Parse(document.ResponseString.Substring(startIdx, endidx - startIdx));
			startIdx = document.ResponseString.IndexOf("<span class=\"b\">", startIdx) + "<span class=\"b\">".Length;
			endidx = document.ResponseString.IndexOf("<", startIdx);
			int average = int.Parse(document.ResponseString.Substring(startIdx, endidx - startIdx));
			Weather weather = new Weather
			                  	{
			                  		day = day,
			                  		from_avg = actual - average
			                  	};
			db.AddToWeathers(weather);
			db.SaveChanges();
			Console.WriteLine(day.ToString("d") + " was off by " + (actual - average) + " degrees on wall street");
		}
	}
}
