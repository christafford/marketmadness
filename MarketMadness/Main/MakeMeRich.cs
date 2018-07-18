using System;
using System.IO;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Persist;
using MarketMadness.DataLayer;

namespace MarketMadness.Main
{
	public class MakeMeRich
	{
		public static void Go()
		{
			var encog = new EncogPersistedCollection("market-network.dat", FileMode.Open);

			Console.WriteLine(@"Loading network");
			var network = (BasicNetwork) encog.Find("market-network");

			Console.WriteLine(@"Reading current data from db");
			var market = new StockMarket();
			market.Init(false);
			var data = market.GetCurrentData();

			Console.WriteLine(@"Running network on data");

			var madness = new ModelMadness();

			foreach (StockMarket.WorkableStockInfo info in data)
			{
				var input = InputOutputMadness.CreateInputs(info);
				var neuralInput = new BasicMLData(input);
				var output = network.Compute(neuralInput);

				Console.WriteLine(@"Stock " + info.ViewToday.stock + @" will change " + output[0] + @"% in the next 20 trading days");

				var future = new prediction
				             	{
				             		day = DateTime.Now.Date,
				             		C20_Days_Close = 100*(decimal) output[0],
				             		stock = info.ViewToday.stock
				             	};

				madness.AddTopredictions(future);
			}

			madness.SaveChanges();

			Console.WriteLine(@"Done - begin making $millions");
		}
	}
}