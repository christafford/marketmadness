using System;
using System.Linq;
using MarketMadness.Agent;
using MarketMadness.DataLayer;

namespace MarketMadness.Scraper
{
	public class ScrapeNYSE
	{
		public static void Start()
		{
			ScrapeNYSE instance = new ScrapeNYSE();
			
			instance.ScrapePage("A");
			instance.ScrapePage("B");
			instance.ScrapePage("C");
			instance.ScrapePage("D");
			instance.ScrapePage("E");
			instance.ScrapePage("F");
			instance.ScrapePage("G");
			instance.ScrapePage("H");
			instance.ScrapePage("I");
			instance.ScrapePage("J");
			instance.ScrapePage("K");
			instance.ScrapePage("L");
			instance.ScrapePage("M");
			instance.ScrapePage("N");
			instance.ScrapePage("O");
			instance.ScrapePage("P");
			instance.ScrapePage("Q");
			instance.ScrapePage("R");
			instance.ScrapePage("S");
			instance.ScrapePage("T");
			instance.ScrapePage("U");
			instance.ScrapePage("V");
			instance.ScrapePage("W");
			instance.ScrapePage("X");
			instance.ScrapePage("Y");
			instance.ScrapePage("Z");
			instance.ScrapePage("Other");

			instance.Madness.SaveChanges();
		}

		protected ModelMadness Madness = new ModelMadness();

		private void ScrapePage(string pageName)
		{
			Console.WriteLine("Doing page " + pageName);

			string url = @"http://www.nyse.com/about/listed/lc_ny_name_`.js".Replace("`", pageName);
			AgentSession session = new AgentSession();
			AgentAction action = new AgentAction(url, false);
			AgentDocument document = AgentHandler.Instance.PerformAction(session, action);

			string[] segments = document.ResponseString.Split(']');

			foreach (string segment in segments)
			{
				int startIdx = segment.IndexOf("\"") + 1;
				if (startIdx == 0)
					continue;
				int endIdx = segment.IndexOf("\"", startIdx);
				string stockTicker = segment.Substring(startIdx, endIdx - startIdx);

				startIdx = segment.IndexOf("\"", endIdx + 1) + 1;
				endIdx = segment.IndexOf("\"", startIdx);

				string companyName = segment.Substring(startIdx, endIdx - startIdx);
				
				startIdx = segment.IndexOf("\"", endIdx + 1) + 1;
				endIdx = segment.IndexOf("\"", startIdx);

				string country = segment.Substring(startIdx, endIdx - startIdx);
;
				StockName name = new StockName
				                 	{
				                 		Stock = stockTicker,
				                 		Company_Name = companyName,
				                 		Country = country,
										Exchange = "NYSE"
				                 	};
				if (! Madness.StockNames.Any(x => x.Stock == stockTicker))
				{
					Console.WriteLine("Found new stock: " + stockTicker);

					Madness.AddToStockNames(name);
				}
			}
		}
	}
}
