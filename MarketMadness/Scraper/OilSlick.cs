using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MarketMadness.Agent;
using MarketMadness.DataLayer;

namespace MarketMadness.Scraper
{
	public class OilSlick
	{
		private static ModelMadness DB = new ModelMadness();
		private static Regex OilSlicker = new Regex(
			"<font size=\"2\" face=\"Verdana, Arial, Helvetica, sans-serif\">\\b([a-z]+)\\b\\s+\\b([0-9]+),\\s([0-9]+)[<>\\s/\"a-z0-9=#,-]+\\$([0-9\\.]+)", 
			RegexOptions.IgnoreCase);
		public static void PerformOilSlick()
		{
			Console.WriteLine("Reading weekly oil prices");
			var document = AgentHandler.Instance.PerformAction(new AgentSession(), new AgentAction("http://www.nyse.tv/crude-oil-price-history.htm", false));
			var match = OilSlicker.Match(document.ResponseString);
			
			while (match.Success)
			{
				var month = match.Result("$1");
				var day = match.Result("$2");
				var year = match.Result("$3");
				var price = decimal.Parse(match.Result("$4"));

				var date = (DateTime) new DateTimeConverter().ConvertFromString(month + " " + day + ", " + year);

				if (DB.CrudeOils.Any(x => x.Day == date))
				{
					Console.WriteLine(date.ToString("MM/dd/yyyy") + " already in database");
				}
				else
				{
					Console.WriteLine("Saving price for " + date.ToString("MM/dd/yyyy") + " - " + price.ToString("c"));
					DB.AddToCrudeOils(new CrudeOil
					                  	{
					                  		Day = date,
					                  		price_at_close = price
					                  	});
				}

				match = match.NextMatch();
			}

			DB.SaveChanges(true);

			Console.WriteLine("Done");
		}
	}
}
