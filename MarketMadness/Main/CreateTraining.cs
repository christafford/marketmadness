using System;
using System.Collections;
using System.IO;
using Encog.ML.Data.Basic;
using Encog.Persist;
using MarketMadness.DataLayer;

namespace MarketMadness.Main
{
	public class CreateTraining
	{
		public static void Create()
		{
			var market = new StockMarket();

			market.Init(true);
			
			Console.WriteLine(@"Fetching training data from db");

			var trainingData = market.GetTrainingData();

			Console.WriteLine(@"Creating training set");

			var inputs = new ArrayList();
			var outputs = new ArrayList();
			var cur = 1;
			
			foreach (var info in trainingData)
			{
				Console.WriteLine(@"Adding record " + (cur ++) + @" of " + trainingData.Count);

				var input = InputOutputMadness.CreateInputs(info);
				var output = new[] { InputOutputMadness.CreateOutput(info) };

				inputs.Add(input);
				outputs.Add(output);
			}

			Console.WriteLine(@"Created training set - saving");

			var trainingSet = new BasicMLDataSet((double[][]) inputs.ToArray(typeof(double[])), (double[][]) outputs.ToArray(typeof(double[])));
			var encog = new EncogPersistedCollection("market-training.dat", FileMode.Create);
			encog.Add("market-training", trainingSet);
			
			Console.WriteLine(@"saved");
		}
	}
}
