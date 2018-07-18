using System;
using System.ComponentModel;
using System.Linq;
using MarketMadness.Agent;
using MarketMadness.DataLayer;

namespace MarketMadness.Scraper
{
	public class GetQuotes
	{
		public static DateTime GetMaxDate()
		{
			ModelMadness madness = new ModelMadness();
			return (from q in madness.quotes orderby q.day descending select q.day).First();
		}

		public static void PullAll(DateTime start, DateTime stop)
		{
			ModelMadness madness = new ModelMadness();

			StockName[] stocks = (from q in madness.StockNames orderby q.Stock select q).ToArray();
			
			foreach (StockName stock in stocks)
				PullQuotes(start, stop, stock.Stock);

			Console.WriteLine("Done");
		}

		public static void PullQuotes(DateTime start, DateTime stop, string stockName)
		{
			ModelMadness madness = new ModelMadness();
			StockName stock = (from q in madness.StockNames where q.Stock == stockName select q).First();

			Console.WriteLine("Working on " + stock.Stock);

			//http://ichart.finance.yahoo.com/table.csv?s=TKR&a=00&b=1&c=2000&d=01&e=18&f=2010&g=d&ignore=.csv

			string url = @"http://ichart.finance.yahoo.com/table.csv?s=" +
			             stock.Stock + "&a=" + TwoDig(start.Month - 1) + "&b=" + start.Day + "&c=" + start.Year +
			             "&d=" + TwoDig(stop.Month - 1) + "&e=" + stop.Day + "&f=" + stop.Year +
			             "&g=d&ignore=.csv";

			AgentSession session = new AgentSession();
			AgentAction action = new AgentAction(url, false);

			bool found = false;
			AgentDocument document = null;

			try
			{
				document = AgentHandler.Instance.PerformAction(session, action);
				found = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + url + " - " + ex.Message);
			}
			
			if (!found)
				return;

			//Date,Open,High,Low,Close,Volume,Adj Close

			string[] rows = document.ResponseString.Split('\n');
			
			if (rows.Length < 2)
				return;

			for (int i = 1; i < rows.Length; i++)
			{
				string row = rows[i];
				string[] fields = row.Split(',');
				if (fields.Length < 7)
				{
					Console.WriteLine((i - 2) + " records stored");
					continue;
				}

				quote q = new quote
				          	{
				          		day = (DateTime) new DateTimeConverter().ConvertFrom(fields[0]),
				          		open = decimal.Parse(fields[1]),
				          		high = decimal.Parse(fields[2]),
				          		low = decimal.Parse(fields[3]),
				          		close = decimal.Parse(fields[4]),
				          		volume = long.Parse(fields[5]),
				          		adjusted_close = decimal.Parse(fields[6]),
								StockName = stock
				          	};

				madness.AddToquotes(q);
			}

			madness.SaveChanges();
		}

		private static string TwoDig(int num)
		{
			if (num > 9)
				return num.ToString();

			return "0" + num;
		}
	}
}
